using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Loader;
using DecayEngine.DecPakLib.Package;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.ResourceBuilderLib.Resource.Compilers;
using DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Font;
using DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Mesh;
using DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Script;
using DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Shader;
using DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.SoundBank;
using DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Texture;

namespace DecayEngine.ResourceBuilderLib.Resource
{
    public static class ResourceCompilationController
    {
        private static readonly List<IResourceCompiler> Compilers;

        static ResourceCompilationController()
        {
            Compilers = new List<IResourceCompiler>
            {
                new ScriptCompiler(),
                new ShaderCompiler(),
                new SoundBankCompiler(),
                new TextureCompiler(),
                new FontCompiler(),
                new MeshCompiler()
            };
        }

        public static void Compile(
            ResourceBundle bundle,
            Uri inputDirectory,
            Uri outputDirectory,
            string bundleFileName = "Resources.decmeta",
            string pakFileNameTemplate = "Resources_{0:D3}.decpak",
            long maxFileSize = 3u * 1024 * 1024 * 1024,
            TextWriter logStream = null)
        {
            if (logStream == null)
            {
                logStream = Console.Out;
            }

            if (!inputDirectory.IsAbsoluteUri)
            {
                inputDirectory = new Uri(new Uri($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}", UriKind.Absolute), inputDirectory);
            }

            if (Path.GetExtension(inputDirectory.LocalPath) != "")
            {
                inputDirectory = new Uri(Path.GetDirectoryName(inputDirectory.LocalPath));
            }
            else if (Path.GetExtension(inputDirectory.LocalPath) == "" &&
                     inputDirectory.LocalPath[inputDirectory.LocalPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                inputDirectory = new Uri($"{inputDirectory.LocalPath}{Path.DirectorySeparatorChar}");
            }

            logStream.WriteLine($"Input directory resolved to: {inputDirectory}.");

            if (!outputDirectory.IsAbsoluteUri)
            {
                outputDirectory = new Uri(new Uri($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}", UriKind.Absolute), outputDirectory);
            }

            if (Path.GetExtension(outputDirectory.LocalPath) != "")
            {
                outputDirectory = new Uri(Path.GetDirectoryName(outputDirectory.LocalPath));
            }
            else if (Path.GetExtension(outputDirectory.LocalPath) == "" &&
                     outputDirectory.LocalPath[outputDirectory.LocalPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                outputDirectory = new Uri($"{outputDirectory.LocalPath}{Path.DirectorySeparatorChar}");
            }

            logStream.WriteLine($"Output directory resolved to: {outputDirectory}.");

            logStream.WriteLine();

            List<DataPointer> writtenPointers = new List<DataPointer>();
            List<DataPackage> packages = new List<DataPackage>();

            foreach (IRootResource rootElement in bundle.Resources.OfType<IRootResource>())
            {
                logStream.WriteLine($"Processing resource: {rootElement.Id}.");
                if (rootElement.Pointers != null && rootElement.Pointers.Count > 0)
                {
                    foreach (ByReference<DataPointer> dataPointer in rootElement.Pointers)
                    {
                        logStream.WriteLine($"Processing data pointer: {dataPointer().SourcePath}.");

                        string metaDir =
                            Path.GetExtension(rootElement.MetaFilePath) != "" ? Path.GetDirectoryName(rootElement.MetaFilePath) : rootElement.MetaFilePath;
                        metaDir = metaDir.TrimEnd('/') + "/";

                        string sourcePath = dataPointer().SourcePath.TrimStart('.').TrimStart('/');
                        Uri sourceUri = new Uri(Path.Combine(metaDir, sourcePath), UriKind.Relative);
                        dataPointer().FullSourcePath = sourceUri.ToString();

                        logStream.WriteLine($"Data pointer path resolved to: {dataPointer().FullSourcePath}.");

                        WriteDataPointer(
                            dataPointer, rootElement,
                            writtenPointers, packages,
                            pakFileNameTemplate, maxFileSize,
                            inputDirectory, outputDirectory,
                            logStream
                        );
                    }
                }
                logStream.WriteLine();
            }

            foreach (DataPackage package in packages.Where(pkg => !pkg.Finalized))
            {
                logStream.WriteLine($"Finalizing package: {package.RelativePath}.");
                using Stream pkgStream = package.Write();

                pkgStream.Seek(pkgStream.Length, SeekOrigin.Begin);
                WritePackageFooter(pkgStream);
            }

            Uri bundleFileUri = new Uri(outputDirectory, bundleFileName);

            if (File.Exists(bundleFileUri.LocalPath))
            {
                logStream.WriteLine($"Deleting existing resource bundle: {bundleFileUri}.");
                File.Delete(bundleFileUri.LocalPath);
            }

            logStream.WriteLine($"Writting resource bundle: {bundleFileUri}.");
            using FileStream bundleFs = File.OpenWrite(bundleFileUri.LocalPath);
            bundle.Serialize(bundleFs);
        }

        public static void Extract(ResourceBundle bundle, Uri inputFile, Uri outputDirectory, TextWriter logStream = null)
        {
            if (logStream == null)
            {
                logStream = Console.Out;
            }

            if (!inputFile.IsAbsoluteUri)
            {
                inputFile = new Uri(new Uri(Directory.GetCurrentDirectory()), inputFile);
            }

            if (Path.GetExtension(inputFile.LocalPath) == "")
            {
                inputFile = new Uri(inputFile, "Resources.decmeta");
            }

            logStream.WriteLine($"Input file resolved to: {inputFile}.");

            if (!outputDirectory.IsAbsoluteUri)
            {
                outputDirectory = new Uri(new Uri(Directory.GetCurrentDirectory()), outputDirectory);
            }

            if (Path.GetExtension(outputDirectory.LocalPath) != "")
            {
                outputDirectory = new Uri(Path.GetDirectoryName(outputDirectory.LocalPath));
            }
            else if (Path.GetExtension(outputDirectory.LocalPath) == "" &&
                     outputDirectory.LocalPath[outputDirectory.LocalPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                outputDirectory = new Uri($"{outputDirectory.LocalPath}{Path.DirectorySeparatorChar}");
            }

            logStream.WriteLine($"Output directory resolved to: {outputDirectory}.");

            logStream.WriteLine();

            List<DataPointer> writtenPointers = new List<DataPointer>();

            foreach (IRootResource rootElement in bundle.Resources.OfType<IRootResource>())
            {
                logStream.WriteLine($"Processing root element: {rootElement.Id}.");
                if (rootElement.Pointers != null && rootElement.Pointers.Count > 0)
                {
                    foreach (ByReference<DataPointer> dataPointer in rootElement.Pointers)
                    {
                        logStream.WriteLine($"Processing data pointer: {dataPointer().SourcePath}.");

                        string metaDir =
                            Path.GetExtension(rootElement.MetaFilePath) != "" ? Path.GetDirectoryName(rootElement.MetaFilePath) : rootElement.MetaFilePath;
                        metaDir = metaDir.TrimEnd('/') + "/";

                        string sourcePath = dataPointer().SourcePath.TrimStart('.').TrimStart('/');
                        Uri sourceUri = new Uri(Path.Combine(metaDir, sourcePath), UriKind.Relative);
                        dataPointer().FullSourcePath = sourceUri.ToString();

                        if (dataPointer().Package.PackageStreamer == null)
                        {
                            Uri inputDirectory = new Uri($"{Path.GetDirectoryName(inputFile.LocalPath)}{Path.DirectorySeparatorChar}", UriKind.Absolute);
                            dataPointer().Package.PackageStreamer = new FilePackageStreamer(inputDirectory);
                        }

                        logStream.WriteLine($"Data pointer path resolved to: {dataPointer().FullSourcePath}.");

                        ExtractDataPointer(dataPointer(), rootElement, writtenPointers, outputDirectory, logStream);
                    }
                }
                logStream.WriteLine();
            }
        }

        public static IResourceCompiler<TResource> GetCompiler<TResource>()
            where TResource : IResource
        {
            return (IResourceCompiler<TResource>) Compilers.FirstOrDefault(value => value is IResourceCompiler<TResource>);
        }

        public static IResourceCompiler GetCompiler(IResource resource)
        {
            return (from serializer in Compilers
                from iface in serializer.GetType().GetInterfaces()
                where iface.IsGenericType
                let genericArg = iface.GetGenericArguments().FirstOrDefault()
                where genericArg != null && genericArg.IsInstanceOfType(resource)
                select serializer).FirstOrDefault();
        }

        private static void WriteDataPointer(
            ByReference<DataPointer> dataPointer, IResource resource,
            ICollection<DataPointer> writtenPointers, ICollection<DataPackage> packages,
            string pakFileNameTemplate, long maxFileSize,
            Uri inputDirectory, Uri outputDirectory,
            TextWriter logStream)
        {
            logStream.WriteLine($"Writting data pointer: {dataPointer().FullSourcePath}.");

            DataPointer existingPointer = writtenPointers.FirstOrDefault(pointer => pointer.FullSourcePath == dataPointer().FullSourcePath);
            if (existingPointer != null)
            {
                logStream.WriteLine("Data pointer already found at: " +
                                    $"{existingPointer.Package.RelativePath} ({existingPointer.Offset}, {existingPointer.Offset + dataPointer().Size}).");

                ref DataPointer pointer = ref dataPointer();
                pointer = existingPointer;
                return;
            }

            DataPackage pkg;
            DataPackage lastPackage = packages.Count > 0 ? packages.Last() : null;
            if (lastPackage == null || lastPackage.Finalized)
            {
                string pkgName = string.Format(pakFileNameTemplate, packages.Count + 1);
                Uri packageUri = new Uri(outputDirectory, pkgName);

                logStream.WriteLine($"Creating new resource package: {packageUri}.");
                pkg = new DataPackage
                {
                    RelativePath = outputDirectory.MakeRelativeUri(packageUri),
                    Size = 0,
                    Finalized = false,
                    PackageStreamer = new FilePackageStreamer(outputDirectory)
                };
                packages.Add(pkg);

                if (File.Exists(packageUri.LocalPath))
                {
                    logStream.WriteLine($"Deleting existing resource package: {packageUri}.");
                    File.Delete(packageUri.LocalPath);
                }
                else
                {
                    string pkgDirectory = Path.GetDirectoryName(packageUri.LocalPath);
                    if (!Directory.Exists(pkgDirectory))
                    {
                        Directory.CreateDirectory(pkgDirectory);
                    }
                }
            }
            else
            {
                logStream.WriteLine($"Appending to existing package: {lastPackage.RelativePath}.");
                pkg = lastPackage;
            }

            List<ByReference<DataPointer>> extraPointers = null;
            using (Stream pkgStream = pkg.Write())
            {
                if (pkg.Size == 0)
                {
                    logStream.WriteLine("Writting package header.");
                    WritePackageHeader(pkgStream);
                }
                else
                {
                    logStream.WriteLine($"Seeking end of package ({pkgStream.Length}).");
                    pkgStream.Seek(pkgStream.Length, SeekOrigin.Begin);
                }

                dataPointer().Offset = pkgStream.Position;
                using (GZipStream compressedPakStream = new GZipStream(pkgStream, CompressionMode.Compress, true))
                {
                    logStream.WriteLine("Compressing data pointer.");
                    using FileStream fileStream = File.OpenRead(new Uri(inputDirectory, dataPointer().FullSourcePath).LocalPath);

                    Stream compiledStream;
                    if (resource != null)
                    {
                        IResourceCompiler compiler = GetCompiler(resource);
                        compiledStream = compiler?.Compile(resource, dataPointer, fileStream, out extraPointers) ?? fileStream;
                    }
                    else
                    {
                        compiledStream = fileStream;
                    }

                    try
                    {
                        compiledStream.Position = 0;
                        compiledStream.CopyTo(compressedPakStream);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error compressing data pointer: {dataPointer().FullSourcePath}.", e);
                    }
                    finally
                    {
                        compiledStream.Close();
                    }
                }
                dataPointer().Size = pkgStream.Position - dataPointer().Offset;

                if (pkgStream.Position >= maxFileSize)
                {
                    logStream.WriteLine("Finalizing package.");
                    WritePackageFooter(pkgStream);
                    pkg.Finalized = true;
                }

                pkg.Size = pkgStream.Position;
            }

            dataPointer().Package = pkg;
            writtenPointers.Add(dataPointer());

            if (extraPointers == null) return;

            foreach (ByReference<DataPointer> extraPointer in extraPointers)
            {
                WriteDataPointer(
                    extraPointer, null,
                    writtenPointers, packages,
                    pakFileNameTemplate, maxFileSize,
                    inputDirectory, outputDirectory,
                    logStream
                );
            }
        }

        private static void ExtractDataPointer(
            DataPointer dataPointer, IResource resource,
            ICollection<DataPointer> writtenPointers,
            Uri outputDirectory, TextWriter logStream)
        {
            bool isExtraPointer = !dataPointer.Valid && new Uri(dataPointer.FullSourcePath).IsAbsoluteUri;
            string extraPointerDirectory = null;
            Uri extraPointerUri = null;

            if (isExtraPointer)
            {
                extraPointerUri = new Uri(outputDirectory, dataPointer.SourcePath);
                extraPointerDirectory = new Uri(Path.GetDirectoryName(extraPointerUri.LocalPath), UriKind.Absolute).LocalPath;
                logStream.WriteLine($"Extracting extra pointer: {outputDirectory.MakeRelativeUri(extraPointerUri)}.");
            }
            else
            {
                logStream.WriteLine($"Extracting data pointer: {dataPointer.FullSourcePath}.");
            }

            DataPointer existingPointer = writtenPointers.FirstOrDefault(pointer => pointer.FullSourcePath == dataPointer.FullSourcePath);
            if (existingPointer != null)
            {
                logStream.WriteLine("Data pointer already extracted, skipping.");
                return;
            }

            List<DataPointer> extraPointers = null;

            if (isExtraPointer)
            {
                using FileStream fs = File.OpenRead(dataPointer.FullSourcePath);

                if (!Directory.Exists(extraPointerDirectory))
                {
                    logStream.WriteLine($"Creating missing directory: {extraPointerDirectory}.");
                    Directory.CreateDirectory(Path.GetDirectoryName(extraPointerUri.LocalPath));
                }

                if (File.Exists(extraPointerUri.LocalPath))
                {
                    logStream.WriteLine($"Deleting existing resource file: {outputDirectory.MakeRelativeUri(extraPointerUri)}.");
                    File.Delete(extraPointerUri.LocalPath);
                }

                using FileStream outFs = File.OpenWrite(extraPointerUri.LocalPath);
                try
                {
                    fs.Position = 0;
                    fs.CopyTo(outFs);

                    writtenPointers.Add(dataPointer);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error extracting data pointer: {outputDirectory.MakeRelativeUri(extraPointerUri)}.", e);
                }
                finally
                {
                    fs.Close();
                    outFs.Close();
                }
            }
            else
            {
                using MemoryStream ms = dataPointer.GetData();

                Stream decompiledStream;
                if (resource != null)
                {
                    IResourceCompiler compiler = GetCompiler(resource);
                    logStream.WriteLine("Decompiling data pointer.");
                    decompiledStream = compiler?.Decompile(resource, dataPointer, ms, out extraPointers) ?? ms;
                }
                else
                {
                    decompiledStream = ms;
                }

                Uri outputPath = new Uri(outputDirectory, dataPointer.FullSourcePath);
                if (!Directory.Exists(Path.GetDirectoryName(outputPath.LocalPath)))
                {
                    logStream.WriteLine($"Creating missing directory: {new Uri(Path.GetDirectoryName(outputPath.LocalPath), UriKind.Absolute)}.");
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath.LocalPath));
                }

                if (File.Exists(outputPath.LocalPath))
                {
                    logStream.WriteLine($"Deleting existing resource file: {outputPath}.");
                    File.Delete(outputPath.LocalPath);
                }

                using FileStream fs = File.OpenWrite(outputPath.LocalPath);
                try
                {
                    decompiledStream.Position = 0;
                    decompiledStream.CopyTo(fs);

                    writtenPointers.Add(dataPointer);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error extracting data pointer: {dataPointer.FullSourcePath}.", e);
                }
                finally
                {
                    decompiledStream.Close();
                }
            }

            if (extraPointers == null) return;

            foreach (DataPointer extraPointer in extraPointers)
            {
                ExtractDataPointer(extraPointer, null, writtenPointers, outputDirectory, logStream);
            }
        }

        private static void WritePackageHeader(Stream stream)
        {
            byte[] header = System.Text.Encoding.UTF8.GetBytes("(DECPAK)");
            byte[] trail = {0x00, 0x00};
            stream.Write(header, 0, header.Length);
            stream.Write(trail, 0, trail.Length);
        }

        private static void WritePackageFooter(Stream stream)
        {
            byte[] footer = System.Text.Encoding.UTF8.GetBytes("(DUNEINTERACTIVE)");
            byte[] lead = {0x00, 0x00};
            stream.Write(lead, 0, lead.Length);
            stream.Write(footer, 0, footer.Length);
        }
    }
}