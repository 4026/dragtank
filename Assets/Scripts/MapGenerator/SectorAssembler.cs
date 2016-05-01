using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragTank.MapGenerator
{
    class SectorAssembler
    {
        private struct SectorChoice
        {
            public Sector sector;
            public IntVector2.Rotation rotation;
        }

        private SectorImporter m_sectorData;

        public SectorAssembler()
        {
            //Load sector data
            m_sectorData = new SectorImporter();
        }

        public void FillMap(Map map)
        {
            IntVector2.Rotation[] rotations = (IntVector2.Rotation[]) Enum.GetValues(typeof(IntVector2.Rotation));

            //Choose sectors to make up the map.
            SectorChoice[,] sector_choices = new SectorChoice[map.Width / Sector.Width, map.Height / Sector.Height];
            for (int x = 0; x < sector_choices.GetLength(0); ++x)
            {
                for (int y = 0; y < sector_choices.GetLength(1); ++y)
                {
                    sector_choices[x, y] = new SectorChoice();
                    sector_choices[x, y].sector = m_sectorData.getRandomMiscSector();
                    sector_choices[x, y].rotation = rotations[UnityEngine.Random.Range(0, rotations.Length)];
                }
            }

            sector_choices[0, 0].sector = m_sectorData.getRandomExitSector();
            sector_choices[sector_choices.GetLength(0) - 1, 0].sector = m_sectorData.getRandomObjectiveSector();
            sector_choices[0, sector_choices.GetLength(1) - 1].sector = m_sectorData.getRandomObjectiveSector();
            sector_choices[sector_choices.GetLength(0) - 1, sector_choices.GetLength(1) - 1].sector = m_sectorData.getRandomObjectiveSector();

            //Write m_sectorData to map.
            for (int x = 0; x < sector_choices.GetLength(0); ++x)
            {
                for (int y = 0; y < sector_choices.GetLength(1); ++y)
                {
                    Sector sector = sector_choices[x, y].sector;
                    IntVector2.Rotation rotation = sector_choices[x, y].rotation;
                    sector.WriteToMap(map, new IntVector2(x * Sector.Width, y * Sector.Height), rotation);
                }
            }
        }
    }
}
