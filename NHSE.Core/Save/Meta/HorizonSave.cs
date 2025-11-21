using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NHSE.Core
{
    /// <summary>
    /// Represents all saved data that is stored on the device for the New Horizon's game.
    /// </summary>
    public class HorizonSave
    {
        public readonly MainSave Main;
        public readonly Player[] Players;
        public readonly string ActiveSaveFolder;
        public readonly IReadOnlyList<string> MirrorSaveFolders;

        public override string ToString() => $"{Players[0].Personal.TownName} - {Players[0]}";

        public HorizonSave(string folder)
        {
            var (primary, mirrors) = ResolveSaveFolders(folder);
            ActiveSaveFolder = primary;
            MirrorSaveFolders = mirrors;

            Main = new MainSave(primary);
            Players = Player.ReadMany(primary);
        }

        /// <summary>
        /// Saves the data using the provided crypto <see cref="seed"/>.
        /// </summary>
        /// <param name="seed">Seed to initialize the RNG with when encrypting the files.</param>
        public void Save(uint seed)
        {
            Main.Hash();
            Main.Save(seed);
            foreach (var player in Players)
            {
                foreach (var pair in player)
                {
                    pair.Hash();
                    pair.Save(seed);
                }
            }

            MirrorSavedFiles();
        }

        /// <summary>
        /// Gets every <see cref="FileHashRegion"/> that is deemed invalid.
        /// </summary>
        /// <remarks>
        /// Doesn't return any metadata about which file the hashes were bad for.
        /// Just check what's returned with what's implemented; the offsets are unique enough.
        /// </remarks>
        public IEnumerable<FileHashRegion> GetInvalidHashes()
        {
            foreach (var hash in Main.InvalidHashes())
                yield return hash;
            foreach (var hash in Players.SelectMany(z => z).SelectMany(z => z.InvalidHashes()))
                yield return hash;
        }

        public void ChangeIdentity(byte[] original, byte[] updated)
        {
            Main.Data.ReplaceOccurrences(original, updated);
            foreach (var pair in Players.SelectMany(z => z))
                pair.Data.ReplaceOccurrences(original, updated);
        }

        public bool ValidateSizes()
        {
            var info = Main.Info.GetKnownRevisionIndex();
            if (info < 0)
                return false;
            var sizes = RevisionChecker.SizeInfo[info];
            if (Main.Data.Length != sizes.Main)
                return false;

            // Each player present in the savedata must have been migrated to this revision.
            foreach (var p in Players)
            {
                if (p.Personal.Data.Length != sizes.Personal)
                    return false;
                if (p.Photo.Data.Length != sizes.PhotoStudioIsland)
                    return false;
                if (p.PostBox.Data.Length != sizes.PostBox)
                    return false;
                if (p.Profile.Data.Length != sizes.Profile)
                    return false;
                if (p.WhereAreN is { } x && x.Data.Length != sizes.WhereAreN)
                    return false;
            }
            return true;
        }

        public string GetSaveTitle(string prefix)
        {
            var townName = Players[0].Personal.TownName;
            var timestamp = Main.LastSaved.TimeStamp;

            return $"{prefix} - {townName} @ {timestamp}";
        }

        public string GetBackupFolderTitle()
        {
            var townName = Players[0].Personal.TownName;
            var timestamp = Main.LastSaved.TimeStamp.Replace(':', '.');
            return StringUtil.CleanFileName($"{townName} - {timestamp}");
        }

        private void MirrorSavedFiles()
        {
            if (MirrorSaveFolders.Count == 0)
                return;

            var primaryFolder = ActiveSaveFolder;
            var files = new List<string>
            {
                Main.DataPath,
                Main.HeaderPath,
            };

            foreach (var pair in Players.SelectMany(z => z))
            {
                files.Add(pair.DataPath);
                files.Add(pair.HeaderPath);
            }

            foreach (var mirror in MirrorSaveFolders)
            {
                foreach (var source in files)
                {
                    var relative = GetRelativePath(primaryFolder, source);
                    var destination = Path.Combine(mirror, relative);
                    var dir = Path.GetDirectoryName(destination);
                    if (dir != null)
                        Directory.CreateDirectory(dir);
                    File.Copy(source, destination, true);
                }
            }
        }

        private static string GetRelativePath(string baseFolder, string fullPath)
        {
            var normalizedBase = EnsureTrailingSeparator(Path.GetFullPath(baseFolder));
            var normalizedFull = Path.GetFullPath(fullPath);
            if (!normalizedFull.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
                return Path.GetFileName(fullPath);

            var relative = normalizedFull.Substring(normalizedBase.Length);
            return relative;
        }

        private static string EnsureTrailingSeparator(string path)
        {
            if (path.Length == 0)
                return path;
            var last = path[path.Length - 1];
            if (last == Path.DirectorySeparatorChar || last == Path.AltDirectorySeparatorChar)
                return path;
            return path + Path.DirectorySeparatorChar;
        }

        private static (string Primary, IReadOnlyList<string> Mirrors) ResolveSaveFolders(string folder)
        {
            var normalizedFolder = Path.GetFullPath(folder);

            if (EncryptedFilePair.Exists(normalizedFolder, "main"))
            {
                var parent = Directory.GetParent(normalizedFolder);
                var siblingMirrors = parent == null
                    ? Array.Empty<string>()
                    : Directory.GetDirectories(parent.FullName, "*", SearchOption.TopDirectoryOnly)
                        .Where(d => !PathsEqual(d, normalizedFolder) && EncryptedFilePair.Exists(d, "main"))
                        .Select(Path.GetFullPath)
                        .ToArray();

                return (normalizedFolder, siblingMirrors);
            }

            var candidates = Directory.GetDirectories(normalizedFolder, "*", SearchOption.TopDirectoryOnly)
                .Where(d => EncryptedFilePair.Exists(d, "main"))
                .Select(d => new
                {
                    Path = Path.GetFullPath(d),
                    Timestamp = File.GetLastWriteTimeUtc(Path.Combine(d, "main.dat")),
                })
                .OrderByDescending(x => x.Timestamp)
                .ToArray();

            if (candidates.Length == 0)
                throw new FileNotFoundException("No save folders containing main.dat were found.", Path.Combine(normalizedFolder, "main.dat"));

            var primary = candidates[0].Path;
            var mirrors = candidates.Skip(1).Select(x => x.Path).ToArray();
            return (primary, mirrors);
        }

        private static bool PathsEqual(string left, string right) =>
            string.Equals(Path.GetFullPath(left).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                          Path.GetFullPath(right).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                          StringComparison.OrdinalIgnoreCase);
    }
}
