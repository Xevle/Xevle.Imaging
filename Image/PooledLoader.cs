using System;
using System.Collections.Generic;

namespace Xevle.Imaging.Image
{
	public class PooledLoader
	{
		int maxImages;
		Dictionary<string, IImage> imgPool;
		List<string> imgLoadOrder;

		public PooledLoader()
		{
			maxImages = 20;
			imgPool = new Dictionary<string, IImage>();
			imgLoadOrder = new List<string>();
		}

		public PooledLoader(uint maxSize)
		{
			maxImages = (int)maxSize;
			imgPool = new Dictionary<string, IImage>();
			imgLoadOrder = new List<string>();
		}

		public IImage FromFile(string filename)
		{
//			if (imgPool.ContainsKey(filename))
//			{
//				imgLoadOrder.Remove(filename);
//				imgLoadOrder.Add(filename);
//				return imgPool[filename];
//			}
//
//			while (maxImages <= imgPool.Count)
//			{
//				string del = imgLoadOrder[0];
//				imgLoadOrder.RemoveAt(0);
//				imgPool.Remove(del);
//			}
//
//			IImage ret = IImage.FromFile(filename);
//			imgPool.Add(filename, ret);
//			imgLoadOrder.Add(filename);
//
//			return ret;

			return null;
		}

		public void Clear()
		{
			imgPool.Clear();
			imgLoadOrder.Clear();
		}

		public int MaxImages
		{
			get
			{
				return maxImages;
			}
			set
			{
				maxImages = value;
			}
		}

		public int Count
		{
			get { return imgLoadOrder.Count; }
		}

		public bool RemoveFileFromPool(string filename)
		{
			string lower = filename.ToLower();

			if (imgPool.ContainsKey(lower))
			{
				imgLoadOrder.Remove(lower);
				imgPool.Remove(lower);
				return true;
			}

			return false;
		}

		public string[] ToArray()
		{
			return imgLoadOrder.ToArray();
		}
	}
}

