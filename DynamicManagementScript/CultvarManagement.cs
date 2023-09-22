/*
1. Code for dynamic cultivar management
This script was used to manage switching crop cultivars, It includes parameters for sowing information, cultivar properties and optional switch buttons to change cultivars
during the simulation. First the script calculates the accumulated rainfall and checks if the crop is alive within the sowing window. If the conditions are met, the crop is 
sown with the specified cultivar and management properties.
The script subscribes to two events: “StartOfSimulation” and “DoManagement”. The “StartOfSimulation” event initializes the accumulatedRain variable with a specified rainfall 
accumulation period, and the “DoManagement” event updates the accumulatedRain variable and checks if the crop needs to be sown based on the sowing window, accumulated rainfall, 
and minimum extractable soil water. If the conditions are met, the script updates the current crop with the specified cultivar and management properties.
The crop cultivar switch are not mandatory although it was in our simulation. If these switches are enabled, the script updates the current crop with the specified cultivar
and management properties at the specified start and end dates within the simulation.

*/

using Models.Interfaces;
using APSIM.Shared.Utilities;
using Models.Utilities;
using Models.Soils;
using Models.PMF;
using Models.Core;
using System;
using Models.Climate;

namespace Models
{
    [Serializable]
    public class Script : Model
    {
        [Link] private Clock Clock;
        [Link] private Summary Summary;
        private Accumulator accumulatedRain;
        [Link]
        private ISoilWater waterBalance;
        
        [Description("Simple Rotation Manager")]
        public Manager Rotations { get; set; }
        
        [Separator("Sowing Info")]
        
        [Description("Crop")]
        public IPlant Crop { get; set; }

        [Description("Start of sowing window (d-mmm)")]
        public string StartDate { get; set; }

        [Description("End of sowing window (d-mmm)")]
        public string EndDate { get; set; }

        [Description("Minimum extractable soil water for sowing (mm)")]
        public double MinESW { get; set; }

        [Description("Accumulated rainfall required for sowing (mm)")]
        public double MinRain { get; set; }

        [Description("Duration of rainfall accumulation (d)")]
        public int RainDays { get; set; }
        
        [Separator("Cultivar  properties")]

        [Display(Type = DisplayType.CultivarName)]
        [Description("Cultivar to be sown")]
        public string CultivarName { get; set; }

        [Description("Sowing depth (mm)")]
        public double SowingDepth { get; set; }

        [Description("Row spacing (mm)")]
        public double RowSpacing { get; set; }

        [Description("Plant population (/m2)")]
        public double Population { get; set; }
        
        [Description("Must sow by end date?")]
        public bool MustSow { get; set; }
        
        [Separator("Optional: only if you want to change cultvar during the simulation")]
        [Description("Switch to another? ")]
        public bool SwitchCultvarOne { get; set; }
        
        [Display(Type = DisplayType.CultivarName)]
        [Description("Select the new cultvar ")]
        public string CultivarNameTwo { get; set; }
        
        [Description("Row spacing (mm)")]
        public double RowSpacingTwo { get; set; }

        [Description("Plant population (/m2)")]
        public double PopulationTwo { get; set; }
        
        [Description("If yes(Ticked), insert starting date (dd/mm/yyyy): ")]
        public DateTime NewCultvarStartDate { get; set; }
        
        [Description("If yes(Ticked), insert starting date (dd/mm/yyyy): ")]
        public DateTime NewCultvarEndDate { get; set; }
        
        [Separator("Optional: if you want to switch to another cultvar again")]
        [Description("Would you like to change cultvar again? ")]
        public bool SwitchCultvarTwo { get; set; }
        
        [Display(Type = DisplayType.CultivarName)]
        [Description("Select the new cultvar ")]
        public string CultivarNameThree { get; set; }
        
        [Description("Row spacing (mm)")]
        public double RowSpacingThree { get; set; }

        [Description("Plant population (/m2)")]
        public double PopulationThree { get; set; }
        
        [Description("If yes(Ticked), insert Start date date (dd/mm/yyyy): ")]
        public DateTime CultvarThreeStartDate { get; set; }
        
        [Description("If yes(Ticked), insert end date date (dd/mm/yyyy): ")]
        public DateTime CultvarThreeEndDate { get; set; }
        
        [Separator("Optional: if you want to switch to another cultvar again")]
        [Description("Would you like to change cultvar again? ")]
        public bool SwitchCultvarThree { get; set; }
        
        [Display(Type = DisplayType.CultivarName)]
        [Description("Select the new cultvar ")]
        public string CultivarNameFive { get; set; }
        
        [Description("Row spacing (mm)")]
        public double RowSpacingFour { get; set; }

        [Description("Plant population (/m2)")]
        public double PopulationFour { get; set; }
        
        [Description("If yes(Ticked), insert Start date date (dd/mm/yyyy): ")]
        public DateTime CultvarFourStartDate { get; set; }
        
        [Description("If yes(Ticked), insert Start date date (dd/mm/yyyy): ")]
        public DateTime CultvarFourEndDate { get; set; }
        
        
        [EventSubscribe("StartOfSimulation")]
        private void OnSimulationCommencing(object sender, EventArgs e)
        {
            accumulatedRain = new Accumulator(this, "[Weather].Rain", RainDays);
        }

        [EventSubscribe("DoManagement")]
        private void OnDoManagement(object sender, EventArgs e)
        {
            accumulatedRain.Update();
                 
                 if (!Crop.IsAlive &&
                DateUtilities.WithinDates(StartDate, Clock.Today, EndDate))
            {
                var rainSum = accumulatedRain.Sum;
                var eswSum  = MathUtilities.Sum(waterBalance.ESW);
                if (rainSum > MinRain && eswSum > MinESW ||
                    DateUtilities.DatesEqual(EndDate, Clock.Today) && MustSow )
                {
                    var thisCrop = Crop.Name.ToLower();
                    var nextCrop = (string)Rotations?.FindByPath("Script.NextCrop")?.Value;
                    if (thisCrop == (nextCrop?.ToLower() ?? thisCrop))
                    if (SwitchCultvarOne && Clock.Today.Date >= NewCultvarStartDate.Date && Clock.Today.Date <= NewCultvarEndDate.Date)
                    {
                        Crop.Sow(population: PopulationTwo,
                                 cultivar: CultivarNameTwo,
                                 depth: SowingDepth,
                                 rowSpacing: RowSpacingTwo);
                      }
                   else if (SwitchCultvarTwo && Clock.Today.Date >= CultvarThreeStartDate.Date && Clock.Today.Date <= CultvarThreeEndDate.Date)
                  {
                  Crop.Sow(population: PopulationThree,
                                 cultivar: CultivarNameThree,
                                 depth: SowingDepth,
                                 rowSpacing: RowSpacingThree);
                  }
                  else if (SwitchCultvarThree && Clock.Today.Date >= CultvarFourStartDate.Date && Clock.Today.Date <= CultvarFourEndDate.Date)
                  {
                  Crop.Sow(population: PopulationFour,
                                 cultivar: CultivarNameThree,
                                 depth: SowingDepth,
                                 rowSpacing: RowSpacingFour);
                  }
                  else
                  {
                  Crop.Sow(population: Population,
                                 cultivar: CultivarName,
                                 depth: SowingDepth,
                                 rowSpacing: RowSpacing);
                  }
                }
        
            }
           
            
        }
    }
}
