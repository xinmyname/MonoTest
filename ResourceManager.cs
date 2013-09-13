using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MonoTest
{
	public class ResourceStore
	{
		private readonly Assembly _resourceAssembly;
		private readonly IList<string> _knownResources;
		private readonly string _rootNamespace;

		public ResourceStore() : 
			this(typeof(ResourceStore).Assembly, 
			     typeof(ResourceStore).Assembly.GetName().Name)
		{
		}

		public ResourceStore(Assembly resourceAssembly, string rootNamespace)
		{
			_resourceAssembly = resourceAssembly;
			_rootNamespace = rootNamespace;
			_knownResources = new List<string>(resourceAssembly.GetManifestResourceNames());
		}

		public string LoadText(string path)
		{
			string name = String.Format("{0}.Resources.{1}", _rootNamespace, path);

			if (!_knownResources.Contains(name))
				throw new MissingResourceException(_resourceAssembly, path);

			var stream = _resourceAssembly.GetManifestResourceStream(name);

			if (stream == null)
				return null;

			var reader = new StreamReader(stream);

			string text = reader.ReadToEnd();

			reader.Close();

			return text;
		}

		public bool Deploy(string resPath, string destPath)
		{
			string name = String.Format("{0}.Resources.{1}", _rootNamespace, resPath);

			if (!_knownResources.Contains(name))
				throw new MissingResourceException(_resourceAssembly, resPath);

			var resStream = _resourceAssembly.GetManifestResourceStream(name);

			if (resStream == null)
				return false;

			Directory.CreateDirectory(Path.GetDirectoryName(destPath));

			using (resStream)
			using (FileStream destStream = new FileStream(destPath, FileMode.Create))
			{
				resStream.CopyTo(destStream);
			}

			return true;
		}

		public class MissingResourceException : ApplicationException
		{
			public MissingResourceException(Assembly assembly, string path)
				: base(String.Format("{0} missing from {1}", path, assembly.FullName))
			{
			}
		}
	}
}