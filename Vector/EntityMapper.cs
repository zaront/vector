using Ank = Anki.Vector.ExternalInterface;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Vector
{
	internal class EntityMapper
	{
		static bool _mappingInit;

		public EntityMapper()
		{
			//setup automapper for mapping to results
			if (!_mappingInit)
			{
				Mapper.Initialize(i =>
				{
					i.CreateMap<Ank.PoseStruct, System.Numerics.Vector3>()
						.ForMember(d => d.X, m => m.MapFrom(s => s.X))
						.ForMember(d => d.Y, m => m.MapFrom(s => s.Y))
						.ForMember(d => d.Z, m => m.MapFrom(s => s.Z));
					i.CreateMap<Ank.PoseStruct, System.Numerics.Quaternion>()
						.ForMember(d => d.W, m => m.MapFrom(s => s.Q0))
						.ForMember(d => d.X, m => m.MapFrom(s => s.Q1))
						.ForMember(d => d.Y, m => m.MapFrom(s => s.Q2))
						.ForMember(d => d.Z, m => m.MapFrom(s => s.Q3));
					i.CreateMap<Ank.PoseStruct, Pose>()
						.ForPath(d => d.Translation, m => m.MapFrom(s => s))
						.ForPath(d => d.Rotation, m => m.MapFrom(s => s));
					i.CreateMap<Ank.CladRect, RectangleF>()
						.ForMember(d => d.X, m => m.MapFrom(s => s.XTopLeft))
						.ForMember(d => d.Y, m => m.MapFrom(s => s.YTopLeft))
						.ForMember(d => d.Width, m => m.MapFrom(s => s.Width))
						.ForMember(d => d.Height, m => m.MapFrom(s => s.Height));

					i.CreateMap<Ank.BatteryStateResponse, BatteryState>();

					i.CreateMap<Ank.VersionStateResponse, VersionState>();

					i.CreateMap<Ank.WakeWord, WakeWord>()
						.ForMember(d => d.IntentHeard, m => m.MapFrom(s => s.WakeWordEnd.IntentHeard))
						.ForMember(d => d.IntentJson, m => m.MapFrom(s => s.WakeWordEnd.IntentJson))
						.ForMember(d => d.Begin, m => m.MapFrom(s => s.WakeWordBegin != null));

					i.CreateMap<Ank.RobotState, RobotState>();

					i.CreateMap<Ank.NavMapFeedResponse, Map>();

					i.CreateMap<Ank.RobotObservedObject, ObservedObject>();

					i.CreateMap<MotionSettings, Ank.PathMotionProfile>();
				});

				_mappingInit = true;
			}
		}

		public T Map<T>(object source)
		{
			return Mapper.Map<T>(source);
		}
	}
}
