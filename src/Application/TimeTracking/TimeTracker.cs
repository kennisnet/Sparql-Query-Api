using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using NLog;

namespace Trezorix.Sparql.Api.Application.TimeTracking
{
	public enum SlotStatus
	{
		Open, Closed
	}

	internal class TimeSlot
	{
		internal long Start;
		internal long End;
		internal string Name;
		internal SlotStatus SlotStatus;
		internal TimeSlot Parent;
		internal List<TimeSlot> _slots = new List<TimeSlot>();

		public TimeSlot(string name, SlotStatus slotStatus, long milliseconds)
		{
			Name = name;
			SlotStatus = slotStatus;
			Start = milliseconds;
		}

		public static TimeSlot New(string name, long milliseconds)
		{
			return new TimeSlot(name, SlotStatus.Open, milliseconds); 
		}

		public TimeSlot AddNew(string name, long milliseconds)
		{
			var ts = new TimeSlot(name, SlotStatus.Open, milliseconds);
			ts.Parent = this;
			_slots.Add(ts);
			return ts;
		}

		public void Close(string name, long milliseconds)
		{
			foreach (var timeSlot in _slots)
			{
				if (timeSlot.SlotStatus == SlotStatus.Open)
				{
					timeSlot.Close(timeSlot.Name, milliseconds);
				}
			}
			End = milliseconds;
			SlotStatus = SlotStatus.Closed;

			if (Name != name)
			{
				if (Parent != null)
				{
					Parent.Close(name, milliseconds);	
				}
			}	
		}

		public void WriteTo(Logger logger, int level = 0)
		{
			string pad = ("").PadLeft(level, ' ');

			if (Parent != null) logger.Info(pad + Name + "-start: " + Start + " ms");
			
			foreach (var timeSlot in _slots)
			{
				timeSlot.WriteTo(logger, level + 1);
			}
			
			if (Parent != null) logger.Info(pad + Name + "-end: " + End + " ms" + " time: " + (End - Start) + " ms");
		}

		public void WriteTo(XmlNode node, int level = 0)
		{
			var root = node;

			if (Parent != null)
			{
				root = node.AppendChild(node.OwnerDocument.CreateElement(Name));
				((XmlElement)root).SetAttribute("start", Start + " ms");
			}

			foreach (var timeSlot in _slots)
			{
				timeSlot.WriteTo(root, level + 1);
			}

			if (Parent != null)
			{
				((XmlElement) root).SetAttribute("end", End + " ms");
				((XmlElement) root).SetAttribute("duration", (End - Start) + " ms");
			}
		}

	}

	public class TimeTracker
	{
		Stopwatch _stopwatch = new Stopwatch();
		private TimeSlot _root = new TimeSlot("", SlotStatus.Open, 0);
		private TimeSlot _current;

		public void Start(string name)
		{
			if (!_stopwatch.IsRunning)
			{
				_stopwatch.Start();
			}
			if (_current == null)
			{
				_current = _root;
			}
			_current = _current.AddNew(name, _stopwatch.ElapsedMilliseconds);
		}

		public void End(string name)
		{
			_current.Close(name, _stopwatch.ElapsedMilliseconds);
			_current = _current.Parent;
		}

		public long TotalTime
		{
			get
			{
				if (_root._slots.Count > 0)
				{
					return _root._slots[0].End - _root._slots[0].Start;
				}
				return 0;
			}
		}

		public void WriteLog(Logger logger)
		{
			_root.WriteTo(logger);
		}

		public void WriteXml(XmlNode appendChild)
		{
			_root.WriteTo(appendChild);
		}
	}
}