using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Data;
using PhasePlayWeb.Models.Entities;
using System.Globalization;
using CsvHelper;
using System.IO;
using System.Threading.Tasks;
using PhasePlayWeb.Services;
using Microsoft.Data.SqlClient;
using Dapper;

namespace PhasePlayWeb.Controllers
{
    public class UploadController : Controller
    {
        private readonly IConfiguration _configuration;

        public UploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GPSDataUpload_GET()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GPSDataUpload_POST(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                
                TempData["ErrorMessage"]= "Please upload a valid CSV file.";
                return RedirectToAction(nameof(GPSDataUpload_GET));
            }

            if (file.ContentType != "text/csv")
            {
               
                TempData["ErrorMessage"] = "Invalid file format. Please upload a CSV file.";
                return RedirectToAction(nameof(GPSDataUpload_GET));
            }

            var connectionString = _configuration.GetConnectionString("WorkOutBuilder");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var stream = new StreamReader(file.OpenReadStream()))
                    {
                        var csvReader = new CsvHelper.CsvReader(stream, CultureInfo.InvariantCulture);
                        csvReader.Context.TypeConverterCache.AddConverter<DateTime?>(new ExcelDateTimeConverter());
                        csvReader.Read();
                        csvReader.ReadHeader();

                        while (csvReader.Read())
                        {
                            var playerGpsData = new PlayerGpsData
                            {
                                Date = csvReader.GetField<DateTime?>("Date"),
                                SessionTitle = csvReader.GetField<string>("Session Title"),
                                PlayerName = csvReader.GetField<string>("Player Name"),
                                SplitName = csvReader.GetField<string>("Split Name"),
                                Tags = csvReader.GetField<string>("Tags"),
                                SplitStartTime = csvReader.GetField<TimeSpan?>("Split Start Time"),
                                SplitEndTime = csvReader.GetField<TimeSpan?>("Split End Time"),
                                DurationSeconds = csvReader.GetField<int?>("Duration"),
                                DistanceMetres = csvReader.GetField<decimal?>("Distance (metres)"),
                                SprintDistanceMetres = csvReader.GetField<decimal?>("Sprint Distance (m)"),
                                PowerPlays = csvReader.GetField<int?>("Power Plays"),
                                EnergyKcal = csvReader.GetField<decimal?>("Energy (kcal)"),
                                Impacts = csvReader.GetField<int?>("Impacts"),
                                HrLoad = csvReader.GetField<decimal?>("Hr Load"),
                                TimeInRedZoneMin = csvReader.GetField<decimal?>("Time In Red Zone (min)"),
                                PlayerLoad = csvReader.GetField<decimal?>("Player Load"),
                                TopSpeedKmh = csvReader.GetField<decimal?>("Top Speed (km/h)"),
                                DistancePerMin = csvReader.GetField<decimal?>("Distance Per Min (m/min)"),
                                PowerScoreWkg = csvReader.GetField<decimal?>("Power Score (w/kg)"),
                                WorkRatio = csvReader.GetField<decimal?>("Work Ratio"),
                                HrMaxBpm = csvReader.GetField<int?>("Hr Max (bpm)"),
                                MaxDecelerationMs2 = csvReader.GetField<decimal?>("Max Deceleration (m/s/s)"),
                                MaxAccelerationMs2 = csvReader.GetField<decimal?>("Max Acceleration (m/s/s)"),
                                DistanceSpeedZone1Metres = csvReader.GetField<decimal?>("Distance in Speed Zone 1  (metres)"),
                                DistanceSpeedZone2Metres = csvReader.GetField<decimal?>("Distance in Speed Zone 2  (metres)"),
                                DistanceSpeedZone3Metres = csvReader.GetField<decimal?>("Distance in Speed Zone 3  (metres)"),
                                DistanceSpeedZone4Metres = csvReader.GetField<decimal?>("Distance in Speed Zone 4  (metres)"),
                                DistanceSpeedZone5Metres = csvReader.GetField<decimal?>("Distance in Speed Zone 5  (metres)"),
                                TimeSpeedZone1Secs = csvReader.GetField<decimal?>("Time in Speed Zone 1 (secs)"),
                                TimeSpeedZone2Secs = csvReader.GetField<decimal?>("Time in Speed Zone 2 (secs)"),
                                TimeSpeedZone3Secs = csvReader.GetField<decimal?>("Time in Speed Zone 3 (secs)"),
                                TimeSpeedZone4Secs = csvReader.GetField<decimal?>("Time in Speed Zone 4 (secs)"),
                                TimeSpeedZone5Secs = csvReader.GetField<decimal?>("Time in Speed Zone 5 (secs)"),
                                ImpactZones3_5GImpacts = csvReader.GetField<int?>("Impact Zones: 3 - 5 G (Impacts)"),
                                ImpactZones5_10GImpacts = csvReader.GetField<int?>("Impact Zones: 5 - 10 G (Impacts)"),
                                ImpactZones10_15GImpacts = csvReader.GetField<int?>("Impact Zones: 10 - 15 G (Impacts)"),
                                ImpactZones15_20GImpacts = csvReader.GetField<int?>("Impact Zones: 15 - 20 G (Impacts)"),
                                ImpactZonesGt20GImpacts = csvReader.GetField<int?>("Impact Zones: > 20 G (Impacts)"),
                                PowerPlayDurationZones0_2_5s = csvReader.GetField<int?>("Power Play Duration Zones: 0 - 2.5 s (Power Plays)"),
                                PowerPlayDurationZones2_5_5s = csvReader.GetField<int?>("Power Play Duration Zones: 2.5 - 5 s (Power Plays)"),
                                PowerPlayDurationZones5_7_5s = csvReader.GetField<int?>("Power Play Duration Zones: 5 - 7.5 s (Power Plays)"),
                                PowerPlayDurationZones7_5_10s = csvReader.GetField<int?>("Power Play Duration Zones: 7.5 - 10 s (Power Plays)"),
                                PowerPlayDurationZonesGt10s = csvReader.GetField<int?>("Power Play Duration Zones: > 10 s (Power Plays)"),
                                DistanceDecelerationZones0_1Ms2Metres = csvReader.GetField<decimal?>("Distance in Deceleration Zones: 0 - 1 m/s/s  (metres)"),
                                DistanceDecelerationZones1_2Ms2Metres = csvReader.GetField<decimal?>("Distance in Deceleration Zones: 1 - 2 m/s/s  (metres)"),
                                DistanceDecelerationZones2_3Ms2Metres = csvReader.GetField<decimal?>("Distance in Deceleration Zones: 2 - 3 m/s/s  (metres)"),
                                DistanceDecelerationZones3_4Ms2Metres = csvReader.GetField<decimal?>("Distance in Deceleration Zones: 3 - 4 m/s/s  (metres)"),
                                DistanceDecelerationZonesGt4Ms2Metres = csvReader.GetField<decimal?>("Distance in Deceleration Zones: > 4 m/s/s  (metres)"),
                                TimeDecelerationZones0_1Ms2Secs = csvReader.GetField<decimal?>("Time in Deceleration Zones: 0 - 1 m/s/s (secs)"),
                                TimeDecelerationZones1_2Ms2Secs = csvReader.GetField<decimal?>("Time in Deceleration Zones: 1 - 2 m/s/s (secs)"),
                                TimeDecelerationZones2_3Ms2Secs = csvReader.GetField<decimal?>("Time in Deceleration Zones: 2 - 3 m/s/s (secs)"),
                                TimeDecelerationZones3_4Ms2Secs = csvReader.GetField<decimal?>("Time in Deceleration Zones: 3 - 4 m/s/s (secs)"),
                                TimeDecelerationZonesGt4Ms2Secs = csvReader.GetField<decimal?>("Time in Deceleration Zones: > 4 m/s/s (secs)"),
                                DistanceAccelerationZones0_1Ms2Metres = csvReader.GetField<decimal?>("Distance in Acceleration Zones: 0 - 1 m/s/s  (metres)"),
                                DistanceAccelerationZones1_2Ms2Metres = csvReader.GetField<decimal?>("Distance in Acceleration Zones: 1 - 2 m/s/s  (metres)"),
                                DistanceAccelerationZones2_3Ms2Metres = csvReader.GetField<decimal?>("Distance in Acceleration Zones: 2 - 3 m/s/s  (metres)"),
                                DistanceAccelerationZones3_4Ms2Metres = csvReader.GetField<decimal?>("Distance in Acceleration Zones: 3 - 4 m/s/s  (metres)"),
                                DistanceAccelerationZonesGt4Ms2Metres = csvReader.GetField<decimal?>("Distance in Acceleration Zones: > 4 m/s/s  (metres)"),
                                TimeAccelerationZones0_1Ms2Secs = csvReader.GetField<decimal?>("Time in Acceleration Zones: 0 - 1 m/s/s (secs)"),
                                TimeAccelerationZones1_2Ms2Secs = csvReader.GetField<decimal?>("Time in Acceleration Zones: 1 - 2 m/s/s (secs)"),
                                TimeAccelerationZones2_3Ms2Secs = csvReader.GetField<decimal?>("Time in Acceleration Zones: 2 - 3 m/s/s (secs)"),
                                TimeAccelerationZones3_4Ms2Secs = csvReader.GetField<decimal?>("Time in Acceleration Zones: 3 - 4 m/s/s (secs)"),
                                TimeAccelerationZonesGt4Ms2Secs = csvReader.GetField<decimal?>("Time in Acceleration Zones: > 4 m/s/s (secs)"),
                                DistancePowerZone0_5WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 0 - 5 w/kg  (metres)"),
                                DistancePowerZone5_10WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 5 - 10 w/kg  (metres)"),
                                DistancePowerZone10_15WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 10 - 15 w/kg  (metres)"),
                                DistancePowerZone15_20WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 15 - 20 w/kg  (metres)"),
                                DistancePowerZone20_25WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 20 - 25 w/kg  (metres)"),
                                DistancePowerZone25_30WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 25 - 30 w/kg  (metres)"),
                                DistancePowerZone30_35WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 30 - 35 w/kg  (metres)"),
                                DistancePowerZone35_40WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 35 - 40 w/kg  (metres)"),
                                DistancePowerZone40_45WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 40 - 45 w/kg  (metres)"),
                                DistancePowerZone45_50WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: 45 - 50 w/kg  (metres)"),
                                DistancePowerZoneGt50WkgMetres = csvReader.GetField<decimal?>("Distance in Power Zone: > 50 w/kg  (metres)"),
                                TimePowerZone0_5WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 0 - 5 w/kg (secs)"),
                                TimePowerZone5_10WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 5 - 10 w/kg (secs)"),
                                TimePowerZone10_15WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 10 - 15 w/kg (secs)"),
                                TimePowerZone15_20WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 15 - 20 w/kg (secs)"),
                                TimePowerZone20_25WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 20 - 25 w/kg (secs)"),
                                TimePowerZone25_30WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 25 - 30 w/kg (secs)"),
                                TimePowerZone30_35WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 30 - 35 w/kg (secs)"),
                                TimePowerZone35_40WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 35 - 40 w/kg (secs)"),
                                TimePowerZone40_45WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 40 - 45 w/kg (secs)"),
                                TimePowerZone45_50WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: 45 - 50 w/kg (secs)"),
                                TimePowerZoneGt50WkgSecs = csvReader.GetField<decimal?>("Time in Power Zone: > 50 w/kg (secs)"),
                                TimeHrLoadZone0_60PercMaxHrSecs = csvReader.GetField<decimal?>("Time in HR Load Zone 0% - 60% Max HR(secs)"),
                                TimeHrLoadZone60_75PercMaxHrSecs = csvReader.GetField<decimal?>("Time in HR Load Zone 60% - 75% Max HR (secs)"),
                                TimeHrLoadZone75_85PercMaxHrSecs = csvReader.GetField<decimal?>("Time in HR Load Zone 75% - 85% Max HR (secs)"),
                                TimeHrLoadZone85_96PercMaxHrSecs = csvReader.GetField<decimal?>("Time in HR Load Zone 85% - 96% Max HR (secs)"),
                                TimeHrLoadZone96_100PercMaxHrSecs = csvReader.GetField<decimal?>("Time in HR Load Zone 96% - 100% Max HR (secs)"),
                                AccelerationsZoneCount0_1Ms2 = csvReader.GetField<int?>("Accelerations Zone Count: 0 - 1 m/s/s"),
                                AccelerationsZoneCount1_2Ms2 = csvReader.GetField<int?>("Accelerations Zone Count: 1 - 2 m/s/s"),
                                AccelerationsZoneCount2_3Ms2 = csvReader.GetField<int?>("Accelerations Zone Count: 2 - 3 m/s/s"),
                                AccelerationsZoneCount3_4Ms2 = csvReader.GetField<int?>("Accelerations Zone Count: 3 - 4 m/s/s"),
                                AccelerationsZoneCountGt4Ms2 = csvReader.GetField<int?>("Accelerations Zone Count: > 4 m/s/s"),
                                DecelerationZoneCount0_1Ms2 = csvReader.GetField<int?>("Deceleration Zone Count: 0 - 1 m/s/s"),
                                DecelerationZoneCount1_2Ms2 = csvReader.GetField<int?>("Deceleration Zone Count: 1 - 2 m/s/s"),
                                DecelerationZoneCount2_3Ms2 = csvReader.GetField<int?>("Deceleration Zone Count: 2 - 3 m/s/s"),
                                DecelerationZoneCount3_4Ms2 = csvReader.GetField<int?>("Deceleration Zone Count: 3 - 4 m/s/s"),
                                DecelerationZoneCountGt4Ms2 = csvReader.GetField<int?>("Deceleration Zone Count: > 4 m/s/s"),
                            };

                            var query = @"
    INSERT INTO uwc_mens_15s.gps_data (
        date, session_title, player_name, split_name, tags, split_start_time, split_end_time, duration_seconds, distance_metres, sprint_distance_metres, power_plays, energy_kcal, impacts, hr_load, time_in_red_zone_min, player_load, top_speed_kmh, distance_per_min, power_score_wkg, work_ratio, hr_max_bpm, max_deceleration_ms2, max_acceleration_ms2, distance_speed_zone1_metres, distance_speed_zone2_metres, distance_speed_zone3_metres, distance_speed_zone4_metres, distance_speed_zone5_metres, time_speed_zone1_secs, time_speed_zone2_secs, time_speed_zone3_secs, time_speed_zone4_secs, time_speed_zone5_secs, impact_zones_3_5G_impacts, impact_zones_5_10G_impacts, impact_zones_10_15G_impacts, impact_zones_15_20G_impacts, impact_zones_gt_20G_impacts, power_play_duration_zones_0_2_5s, power_play_duration_zones_2_5_5s, power_play_duration_zones_5_7_5s, power_play_duration_zones_7_5_10s, power_play_duration_zones_gt_10s, distance_deceleration_zones_0_1_ms2_metres, distance_deceleration_zones_1_2_ms2_metres, distance_deceleration_zones_2_3_ms2_metres, distance_deceleration_zones_3_4_ms2_metres, distance_deceleration_zones_gt_4_ms2_metres, time_deceleration_zones_0_1_ms2_secs, time_deceleration_zones_1_2_ms2_secs, time_deceleration_zones_2_3_ms2_secs, time_deceleration_zones_3_4_ms2_secs, time_deceleration_zones_gt_4_ms2_secs, distance_acceleration_zones_0_1_ms2_metres, distance_acceleration_zones_1_2_ms2_metres, distance_acceleration_zones_2_3_ms2_metres, distance_acceleration_zones_3_4_ms2_metres, distance_acceleration_zones_gt_4_ms2_metres, time_acceleration_zones_0_1_ms2_secs, time_acceleration_zones_1_2_ms2_secs, time_acceleration_zones_2_3_ms2_secs, time_acceleration_zones_3_4_ms2_secs, time_acceleration_zones_gt_4_ms2_secs, distance_power_zone_0_5_wkg_metres, distance_power_zone_5_10_wkg_metres, distance_power_zone_10_15_wkg_metres, distance_power_zone_15_20_wkg_metres, distance_power_zone_20_25_wkg_metres, distance_power_zone_25_30_wkg_metres, distance_power_zone_30_35_wkg_metres, distance_power_zone_35_40_wkg_metres, distance_power_zone_40_45_wkg_metres, distance_power_zone_45_50_wkg_metres, distance_power_zone_gt_50_wkg_metres, time_power_zone_0_5_wkg_secs, time_power_zone_5_10_wkg_secs, time_power_zone_10_15_wkg_secs, time_power_zone_15_20_wkg_secs, time_power_zone_20_25_wkg_secs, time_power_zone_25_30_wkg_secs, time_power_zone_30_35_wkg_secs, time_power_zone_35_40_wkg_secs, time_power_zone_40_45_wkg_secs, time_power_zone_45_50_wkg_secs, time_power_zone_gt_50_wkg_secs, time_hr_load_zone_0_60_perc_max_hr_secs, time_hr_load_zone_60_75_perc_max_hr_secs, time_hr_load_zone_75_85_perc_max_hr_secs, time_hr_load_zone_85_96_perc_max_hr_secs, time_hr_load_zone_96_100_perc_max_hr_secs, accelerations_zone_count_0_1_ms2, accelerations_zone_count_1_2_ms2, accelerations_zone_count_2_3_ms2, accelerations_zone_count_3_4_ms2, accelerations_zone_count_gt_4_ms2, deceleration_zone_count_0_1_ms2, deceleration_zone_count_1_2_ms2, deceleration_zone_count_2_3_ms2, deceleration_zone_count_3_4_ms2, deceleration_zone_count_gt_4_ms2
    ) VALUES (
        @Date, @SessionTitle, @PlayerName, @SplitName, @Tags, @SplitStartTime, @SplitEndTime, @DurationSeconds, @DistanceMetres, @SprintDistanceMetres, @PowerPlays, @EnergyKcal, @Impacts, @HrLoad, @TimeInRedZoneMin, @PlayerLoad, @TopSpeedKmh, @DistancePerMin, @PowerScoreWkg, @WorkRatio, @HrMaxBpm, @MaxDecelerationMs2, @MaxAccelerationMs2, @DistanceSpeedZone1Metres, @DistanceSpeedZone2Metres, @DistanceSpeedZone3Metres, @DistanceSpeedZone4Metres, @DistanceSpeedZone5Metres, @TimeSpeedZone1Secs, @TimeSpeedZone2Secs, @TimeSpeedZone3Secs, @TimeSpeedZone4Secs, @TimeSpeedZone5Secs, @ImpactZones3_5GImpacts, @ImpactZones5_10GImpacts, @ImpactZones10_15GImpacts, @ImpactZones15_20GImpacts, @ImpactZonesGt20GImpacts, @PowerPlayDurationZones0_2_5s, @PowerPlayDurationZones2_5_5s, @PowerPlayDurationZones5_7_5s, @PowerPlayDurationZones7_5_10s, @PowerPlayDurationZonesGt10s, @DistanceDecelerationZones0_1Ms2Metres, @DistanceDecelerationZones1_2Ms2Metres, @DistanceDecelerationZones2_3Ms2Metres, @DistanceDecelerationZones3_4Ms2Metres, @DistanceDecelerationZonesGt4Ms2Metres, @TimeDecelerationZones0_1Ms2Secs, @TimeDecelerationZones1_2Ms2Secs, @TimeDecelerationZones2_3Ms2Secs, @TimeDecelerationZones3_4Ms2Secs, @TimeDecelerationZonesGt4Ms2Secs, @DistanceAccelerationZones0_1Ms2Metres, @DistanceAccelerationZones1_2Ms2Metres, @DistanceAccelerationZones2_3Ms2Metres, @DistanceAccelerationZones3_4Ms2Metres, @DistanceAccelerationZonesGt4Ms2Metres, @TimeAccelerationZones0_1Ms2Secs, @TimeAccelerationZones1_2Ms2Secs, @TimeAccelerationZones2_3Ms2Secs, @TimeAccelerationZones3_4Ms2Secs, @TimeAccelerationZonesGt4Ms2Secs, @DistancePowerZone0_5WkgMetres, @DistancePowerZone5_10WkgMetres, @DistancePowerZone10_15WkgMetres, @DistancePowerZone15_20WkgMetres, @DistancePowerZone20_25WkgMetres, @DistancePowerZone25_30WkgMetres, @DistancePowerZone30_35WkgMetres, @DistancePowerZone35_40WkgMetres, @DistancePowerZone40_45WkgMetres, @DistancePowerZone45_50WkgMetres, @DistancePowerZoneGt50WkgMetres, @TimePowerZone0_5WkgSecs, @TimePowerZone5_10WkgSecs, @TimePowerZone10_15WkgSecs, @TimePowerZone15_20WkgSecs, @TimePowerZone20_25WkgSecs, @TimePowerZone25_30WkgSecs, @TimePowerZone30_35WkgSecs, @TimePowerZone35_40WkgSecs, @TimePowerZone40_45WkgSecs, @TimePowerZone45_50WkgSecs, @TimePowerZoneGt50WkgSecs, @TimeHrLoadZone0_60PercMaxHrSecs, @TimeHrLoadZone60_75PercMaxHrSecs, @TimeHrLoadZone75_85PercMaxHrSecs, @TimeHrLoadZone85_96PercMaxHrSecs, @TimeHrLoadZone96_100PercMaxHrSecs, @AccelerationsZoneCount0_1Ms2, @AccelerationsZoneCount1_2Ms2, @AccelerationsZoneCount2_3Ms2, @AccelerationsZoneCount3_4Ms2, @AccelerationsZoneCountGt4Ms2, @DecelerationZoneCount0_1Ms2, @DecelerationZoneCount1_2Ms2, @DecelerationZoneCount2_3Ms2, @DecelerationZoneCount3_4Ms2, @DecelerationZoneCountGt4Ms2
    )";

                            await connection.ExecuteAsync(query, playerGpsData);

                        }

                        
                    }
                    TempData["SuccessMessage"] = "Your Data have been successfully uploaded!";
                    return RedirectToAction(nameof(GPSDataUpload_GET));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["ErrorMessage"]= ex.Message;
                return RedirectToAction(nameof(GPSDataUpload_GET));
            }
        }

    }
}