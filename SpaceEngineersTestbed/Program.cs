using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.

            List<IMyRadioAntenna> Antennalist = new List<IMyRadioAntenna>();
            GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(Antennalist); //puts all antennas in a list
            List<IMyRemoteControl> Remotelist = new List<IMyRemoteControl>();
            GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(Remotelist); //puts all remotes in a list
            List<IMyTextSurface> myTextSurfaces = new List<IMyTextSurface>();
            GridTerminalSystem.GetBlocksOfType<IMyTextSurface>(myTextSurfaces); //puts all small lcds in list
            List<IMySensorBlock> Sensorlist = new List<IMySensorBlock>();
            GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(Sensorlist); //puts sensors in list

            //need to fly to a waypoint, then enable sensors, set ranges to 1, then ramp up range to max limit, adding anything found to a list of entities.
            //Then transmit list of entities back to ship and show on lcd; program block lcd should work. Try to also set global gps waypoints for each item.


            //check for at least one block of each type first.
            if(Antennalist == null || Remotelist == null || myTextSurfaces == null || Sensorlist == null)
            {
                Echo("Missing Blocks! Need Remote, Antenna, Sensor & Program LCD!");
            }
            else
            {
                //focus on the sensors. Lets get it setting the range and creating gps points.
                foreach (IMySensorBlock sensor in Sensorlist)
                {
                    sensor.LeftExtend = 1000;
                    sensor.RightExtend = 1000;
                    sensor.TopExtend = 1000;
                    sensor.BottomExtend = 1000;
                    sensor.FrontExtend = 1000;
                    sensor.BackExtend = 1000;
                    
                }
                List<MyDetectedEntityInfo> detectedEntities = new List<MyDetectedEntityInfo>();
                foreach (IMySensorBlock sensor in Sensorlist)
                {
                    List<MyDetectedEntityInfo> myentities = new List<MyDetectedEntityInfo>();
                    sensor.DetectedEntities(myentities);
                    foreach (MyDetectedEntityInfo info in myentities)
                    {
                        detectedEntities.Add(info);
                    }
                }
                
                List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
                foreach (MyDetectedEntityInfo info in detectedEntities)
                {
                    
                    waypoints.Add(new MyWaypointInfo(info.Name, info.Position));
                }
                Echo(waypoints.Count().ToString() + " Waypoints Created");
                foreach (MyWaypointInfo waypoint in waypoints)
                {
                    List<MyWaypointInfo> knownwaypoints = new List<MyWaypointInfo>();
                    Remotelist[0].GetWaypointInfo(knownwaypoints);
                    if (knownwaypoints.Contains(waypoint)) { }
                    else
                    {
                        Remotelist[0].AddWaypoint(waypoint);
                    }
                }
                Echo("Waypoints Added");
                
            }
        }
    }
}
