using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PhasePlayWeb.Models.Entities
{
    [Table("gps_data", Schema = "uwc_mens_15s")]
    [Keyless]
    public class PlayerGpsData
    {
      
        [Column("date")]
        public DateTime? Date { get; set; }

        [Column("[[session title]]")]
        [MaxLength(255)]
        public string? SessionTitle { get; set; }

        [Column("player_name")]
        [MaxLength(255)]
        public string? PlayerName { get; set; }

        [Column("[[split name]]")]
        [MaxLength(255)]
        public string? SplitName { get; set; }

        [Column("tags")]
        [MaxLength(255)]
        public string? Tags { get; set; }

        [Column("[[split start time]]")]
        public TimeSpan? SplitStartTime { get; set; }

        [Column("[[split end time]]")]
        public TimeSpan? SplitEndTime { get; set; }

        [Column("duration_seconds")]
        public int? DurationSeconds { get; set; }

        [Column("distance_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceMetres { get; set; }

        [Column("sprint_distance_metres", TypeName = "decimal(10, 2)")]
        public decimal? SprintDistanceMetres { get; set; }

        [Column("power_plays")]
        public int? PowerPlays { get; set; }

        [Column("energy_kcal", TypeName = "decimal(10, 2)")]
        public decimal? EnergyKcal { get; set; }

        [Column("impacts")]
        public int? Impacts { get; set; }

        [Column("hr_load", TypeName = "decimal(10, 2)")]
        public decimal? HrLoad { get; set; }

        [Column("time_in_red_zone_min", TypeName = "decimal(10, 2)")]
        public decimal? TimeInRedZoneMin { get; set; }

        [Column("player_load", TypeName = "decimal(10, 2)")]
        public decimal? PlayerLoad { get; set; }

        [Column("top_speed_kmh", TypeName = "decimal(10, 2)")]
        public decimal? TopSpeedKmh { get; set; }

        [Column("distance_per_min", TypeName = "decimal(10, 2)")]
        public decimal? DistancePerMin { get; set; }

        [Column("power_score_wkg", TypeName = "decimal(10, 2)")]

        public decimal? PowerScoreWkg { get; set; }

        [Column("work_ratio", TypeName = "decimal(10, 2)")]
        public decimal? WorkRatio { get; set; }

        [Column("hr_max_bpm")]
        public int? HrMaxBpm { get; set; }

        [Column("max_deceleration_ms2", TypeName = "decimal(10, 2)")]
        public decimal? MaxDecelerationMs2 { get; set; }

        [Column("max_acceleration_ms2", TypeName = "decimal(10, 2)")]
        public decimal? MaxAccelerationMs2 { get; set; }

        [Column("distance_speed_zone1_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceSpeedZone1Metres { get; set; }

        [Column("distance_speed_zone2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceSpeedZone2Metres { get; set; }

        [Column("distance_speed_zone3_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceSpeedZone3Metres { get; set; }

        [Column("distance_speed_zone4_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceSpeedZone4Metres { get; set; }

        [Column("distance_speed_zone5_metres", TypeName = "decimal( 10, 2)")]
        public decimal? DistanceSpeedZone5Metres { get; set; }

        [Column("time_speed_zone1_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeSpeedZone1Secs { get; set; }

        [Column("time_speed_zone2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeSpeedZone2Secs { get; set; }

        [Column("time_speed_zone3_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeSpeedZone3Secs { get; set; }

        [Column("time_speed_zone4_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeSpeedZone4Secs { get; set; }

        [Column("time_speed_zone5_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeSpeedZone5Secs { get; set; }

        [Column("impact_zones_3_5G_impacts")]
        public int? ImpactZones3_5GImpacts { get; set; }

        [Column("impact_zones_5_10G_impacts")]
        public int? ImpactZones5_10GImpacts { get; set; }

        [Column("impact_zones_10_15G_impacts")]
        public int? ImpactZones10_15GImpacts { get; set; }

        [Column("impact_zones_15_20G_impacts")]
        public int? ImpactZones15_20GImpacts { get; set; }

        [Column("impact_zones_gt_20G_impacts")]
        public int? ImpactZonesGt20GImpacts { get; set; }

        [Column("power_play_duration_zones_0_2_5s")]
        public int? PowerPlayDurationZones0_2_5s { get; set; }

        [Column("power_play_duration_zones_2_5_5s")]
        public int? PowerPlayDurationZones2_5_5s { get; set; }

        [Column("power_play_duration_zones_5_7_5s")]
        public int? PowerPlayDurationZones5_7_5s { get; set; }

        [Column("power_play_duration_zones_7_5_10s")]
        public int? PowerPlayDurationZones7_5_10s { get; set; }

        [Column("power_play_duration_zones_gt_10s")]
        public int? PowerPlayDurationZonesGt10s { get; set; }

        [Column("distance_deceleration_zones_0_1_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceDecelerationZones0_1Ms2Metres { get; set; }

        [Column("distance_deceleration_zones_1_2_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceDecelerationZones1_2Ms2Metres { get; set; }

        [Column("distance_deceleration_zones_2_3_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceDecelerationZones2_3Ms2Metres { get; set; }

        [Column("distance_deceleration_zones_3_4_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceDecelerationZones3_4Ms2Metres { get; set; }

        [Column("distance_deceleration_zones_gt_4_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceDecelerationZonesGt4Ms2Metres { get; set; }

        [Column("time_deceleration_zones_0_1_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeDecelerationZones0_1Ms2Secs { get; set; }

        [Column("time_deceleration_zones_1_2_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeDecelerationZones1_2Ms2Secs { get; set; }

        [Column("time_deceleration_zones_2_3_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeDecelerationZones2_3Ms2Secs { get; set; }

        [Column("time_deceleration_zones_3_4_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeDecelerationZones3_4Ms2Secs { get; set; }

        [Column("time_deceleration_zones_gt_4_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeDecelerationZonesGt4Ms2Secs { get; set; }

        [Column("distance_acceleration_zones_0_1_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceAccelerationZones0_1Ms2Metres { get; set; }

        [Column("distance_acceleration_zones_1_2_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceAccelerationZones1_2Ms2Metres { get; set; }

        [Column("distance_acceleration_zones_2_3_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceAccelerationZones2_3Ms2Metres { get; set; }

        [Column("distance_acceleration_zones_3_4_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceAccelerationZones3_4Ms2Metres { get; set; }

        [Column("distance_acceleration_zones_gt_4_ms2_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistanceAccelerationZonesGt4Ms2Metres { get; set; }

        [Column("time_acceleration_zones_0_1_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeAccelerationZones0_1Ms2Secs { get; set; }

        [Column("time_acceleration_zones_1_2_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeAccelerationZones1_2Ms2Secs { get; set; }

        [Column("time_acceleration_zones_2_3_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeAccelerationZones2_3Ms2Secs { get; set; }

        [Column("time_acceleration_zones_3_4_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeAccelerationZones3_4Ms2Secs { get; set; }

        [Column("time_acceleration_zones_gt_4_ms2_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeAccelerationZonesGt4Ms2Secs { get; set; }

        [Column("distance_power_zone_0_5_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone0_5WkgMetres { get; set; }

        [Column("distance_power_zone_5_10_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone5_10WkgMetres { get; set; }

        [Column("distance_power_zone_10_15_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone10_15WkgMetres { get; set; }

        [Column("distance_power_zone_15_20_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone15_20WkgMetres { get; set; }

        [Column("distance_power_zone_20_25_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone20_25WkgMetres { get; set; }

        [Column("distance_power_zone_25_30_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone25_30WkgMetres { get; set; }

        [Column("distance_power_zone_30_35_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone30_35WkgMetres { get; set; }

        [Column("distance_power_zone_35_40_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone35_40WkgMetres { get; set; }

        [Column("distance_power_zone_40_45_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone40_45WkgMetres { get; set; }

        [Column("distance_power_zone_45_50_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZone45_50WkgMetres { get; set; }

        [Column("distance_power_zone_gt_50_wkg_metres", TypeName = "decimal(10, 2)")]
        public decimal? DistancePowerZoneGt50WkgMetres { get; set; }

        [Column("time_power_zone_0_5_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone0_5WkgSecs { get; set; }

        [Column("time_power_zone_5_10_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone5_10WkgSecs { get; set; }

        [Column("time_power_zone_10_15_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone10_15WkgSecs { get; set; }

        [Column("time_power_zone_15_20_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone15_20WkgSecs { get; set; }

        [Column("time_power_zone_20_25_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone20_25WkgSecs { get; set; }

        [Column("time_power_zone_25_30_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone25_30WkgSecs { get; set; }

        [Column("time_power_zone_30_35_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone30_35WkgSecs { get; set; }

        [Column("time_power_zone_35_40_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone35_40WkgSecs { get; set; }

        [Column("time_power_zone_40_45_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone40_45WkgSecs { get; set; }

        [Column("time_power_zone_45_50_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZone45_50WkgSecs { get; set; }

        [Column("time_power_zone_gt_50_wkg_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimePowerZoneGt50WkgSecs { get; set; }

        [Column("time_hr_load_zone_0_60_perc_max_hr_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeHrLoadZone0_60PercMaxHrSecs { get; set; }

        [Column("time_hr_load_zone_60_75_perc_max_hr_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeHrLoadZone60_75PercMaxHrSecs { get; set; }

        [Column("time_hr_load_zone_75_85_perc_max_hr_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeHrLoadZone75_85PercMaxHrSecs { get; set; }

        [Column("time_hr_load_zone_85_96_perc_max_hr_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeHrLoadZone85_96PercMaxHrSecs { get; set; }

        [Column("time_hr_load_zone_96_100_perc_max_hr_secs", TypeName = "decimal(10, 2)")]
        public decimal? TimeHrLoadZone96_100PercMaxHrSecs { get; set; }

        [Column("accelerations_zone_count_0_1_ms2")]
        public int? AccelerationsZoneCount0_1Ms2 { get; set; }

        [Column("accelerations_zone_count_1_2_ms2")]
        public int? AccelerationsZoneCount1_2Ms2 { get; set; }

        [Column("accelerations_zone_count_2_3_ms2")]
        public int? AccelerationsZoneCount2_3Ms2 { get; set; }

        [Column("accelerations_zone_count_3_4_ms2")]
        public int? AccelerationsZoneCount3_4Ms2 { get; set; }

        [Column("accelerations_zone_count_gt_4_ms2")]
        public int? AccelerationsZoneCountGt4Ms2 { get; set; }

        [Column("deceleration_zone_count_0_1_ms2")]
        public int? DecelerationZoneCount0_1Ms2 { get; set; }

        [Column("deceleration_zone_count_1_2_ms2")]
        public int? DecelerationZoneCount1_2Ms2 { get; set; }

        [Column("deceleration_zone_count_2_3_ms2")]
        public int? DecelerationZoneCount2_3Ms2 { get; set; }

        [Column("deceleration_zone_count_3_4_ms2")]
        public int? DecelerationZoneCount3_4Ms2 { get; set; }

        [Column("deceleration_zone_count_gt_4_ms2")]
        public int? DecelerationZoneCountGt4Ms2 { get; set; }

        [Column("max_deceleration", TypeName = "decimal(10, 2)")]
        public decimal? MaxDeceleration { get; set; }

        [Column("max_acceleration", TypeName = "decimal(10, 2)")]
        public decimal? MaxAcceleration { get; set; }

        [Column("METS", TypeName = "decimal(10, 2)")]
        public decimal? Mets { get; set; }

        [Column("duration_time")]
        public TimeSpan? DurationTime { get; set; }

        [Column("player_id")]
        public int? PlayerId { get; set; }
    }
}
