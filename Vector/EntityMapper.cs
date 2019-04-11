using Anki.Vector.ExternalInterface;
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
					i.CreateMap<BatteryStateResponse, BatteryState>();

					i.CreateMap<NetworkStateResponse, NetworkState>();

					i.CreateMap<VersionStateResponse, VersionState>();

					i.CreateMap<Anki.Vector.ExternalInterface.WakeWord, WakeWord>()
						.ForMember(d => d.IntentHeard, m => m.MapFrom(s => s.WakeWordEnd.IntentHeard))
						.ForMember(d => d.IntentJson, m => m.MapFrom(s => s.WakeWordEnd.IntentJson))
						.ForMember(d => d.Begin, m => m.MapFrom(s => s.WakeWordBegin != null));

					i.CreateMap<Anki.Vector.ExternalInterface.RobotState, RobotState>();

					i.CreateMap<NavMapFeedResponse, Map>();

					i.CreateMap<RobotObservedObject, ObservedObject>();
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
