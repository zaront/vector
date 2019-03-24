using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
	public class NetworkState
	{
		NetworkStats NetworkStats;
	}

	public class NetworkStats
	{
		public int g_net_stat1_num_connections;
		public float g_net_stat2_latency_avg;
		public float g_net_stat3_latency_sd;
		public float g_net_stat4_latency_min;
		public float g_net_stat5_latency_max;
		public float g_net_stat6_ping_arrived_pc;
		public float g_net_stat7_ext_queued_avg_ms;
		public float g_net_stat8_ext_queued_min_ms;
		public float g_net_stat9_ext_queued_max_ms;
		public float g_net_stata_queued_avg_ms;
		public float g_net_statb_queued_min_ms;
		public float g_net_statc_queued_max_ms;
	}
}
