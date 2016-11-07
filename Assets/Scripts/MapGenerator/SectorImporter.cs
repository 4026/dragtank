using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DragTank.MapGenerator
{
    /// <summary>
    /// Reads in the sector data file and instantiates Sector objects from it.
    /// </summary>
    class SectorImporter
    {
        private List<Sector> m_entranceSectors = new List<Sector>();
        private List<Sector> m_exitSectors = new List<Sector>();
        private List<Sector> m_objectiveSectors = new List<Sector>();
        private List<Sector> m_miscSectors = new List<Sector>();

        public SectorImporter()
        {
            TextAsset file = Resources.Load<TextAsset>("sectors");

            string[] sector_strings = file.text.Split(
                new String[] { "\r\n\r\n", Environment.NewLine + Environment.NewLine }, 
                1000, 
                StringSplitOptions.RemoveEmptyEntries
            );

            for (int i = 0; i < sector_strings.Length; ++i)
            {
                string sector_string = sector_strings[i];

                try
                {
                    Sector sector = new Sector(sector_string);

                    if (sector.ContainsEntrance)
                    {
                        m_entranceSectors.Add(sector);
                    }
                    else if (sector.ContainsExit)
                    {
                        m_exitSectors.Add(sector);
                    }
                    else if (sector.ContainsObjective)
                    {
                        m_objectiveSectors.Add(sector);
                    }
                    else
                    {
                        m_miscSectors.Add(sector);
                    }
                    
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Caught exception while parsing data for sector {0}: {1}", i, e.Message);
                }
            }

        }

        public Sector getRandomEntranceSector()
        {
            return m_entranceSectors[UnityEngine.Random.Range(0, m_entranceSectors.Count)];
        }

        public Sector getRandomExitSector()
        {
            return m_exitSectors[UnityEngine.Random.Range(0, m_exitSectors.Count)];
        }

        public Sector getRandomObjectiveSector()
        {
            return m_objectiveSectors[UnityEngine.Random.Range(0, m_objectiveSectors.Count)];
        }

        public Sector getRandomMiscSector()
        {
            return m_miscSectors[UnityEngine.Random.Range(0, m_miscSectors.Count)];
        }
    }
}
