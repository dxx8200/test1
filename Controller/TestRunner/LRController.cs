using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wlrun;
using System.Runtime.InteropServices;

namespace Controller.TestRunner
{
    public class LRController
    {
        private const string LOADGENERATOR = "localhost";
        private const string GROUPNAME = "sock_group";

        private static ILrEngine _engine = null;

        public static int OpenLR()
        {
            _engine = new LrEngine();
            //_engine.ShowMainWindow(0);
            
            while (true)
            {
                try { ILrScenario scenario = _engine.Scenario; break; }
                catch (Exception) { }
            }
            return _engine.GetMainWindow();          
        }

        public static void InitScenario(IList<string> vUserPath)
        {
            if (_engine == null)
            {
                throw new Exception();
            }
            ILrScenario scenario = _engine.Scenario;
            if (scenario.IsOpened())
            {
                scenario.Reset();// throw new Exception("Scenario has Opened");
            }
            scenario.New(false, LrScenarioType.lrVusers);
            scenario.SetAttrib(LrScAttribs.lrScAutoOverwriteResult, 1);

            InitHost(scenario);

            int vUserNumber = vUserPath.Count;
            _engine.GetVuserTypeLicenseLimit("", vUserNumber);

            for (int i = 0; i < vUserPath.Count;  i++)
            {
                String ScriptName = "sock_script" + i.ToString();
                String GroupName = GROUPNAME + i.ToString();
                try
                {
                    scenario.Scripts.get_Item(ScriptName);
                    scenario.Scripts.Remove(ScriptName);
                }
                catch (Exception) { }
              
                try
                {
                    scenario.Scripts.Add(vUserPath[i], ScriptName);
                    scenario.Scripts.get_Item(ScriptName);
                    scenario.Groups.Add(GroupName);
                    scenario.Groups.get_Item(GroupName).AddVusers( ScriptName, LOADGENERATOR, 1);

                }
                catch (Exception)
                {
                	throw new Exception("Add Script Failed.");
                }
            }
            
        }

        public static void Start()
        {
            _engine.Scenario.Start();
        }

        public static bool IsRunning()
        {
            try
            {
                return _engine.Scenario.DidScenarioRun();
            }
            catch (Exception){}
            return false;
        }

        public static void Stop()
        {
            _engine.Scenario.Stop();
        }

        public static void CloseLR()
        {
            _engine.CloseController();
            _engine = null;
        }

        private static void InitHost(ILrScenario scenario)
        {
            ILrHost host = scenario.Hosts.get_Item(LOADGENERATOR);
            
            if (host == null)
            {
                scenario.Hosts.Add(LOADGENERATOR, LrHostPlatform.lrWINDOWS);
                host = scenario.Hosts[LOADGENERATOR];

            }
            host.Connect();
        }
    }
}
