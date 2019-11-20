using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointCloud.Exceptions
{
	class PCDReaderException : PointCloudException
	{
		public PCDReaderException () {}

		public PCDReaderException(String message) : base(message)
		{
		}
	}
}