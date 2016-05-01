using DragTank.Extensions;
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
            public Sector Sector;
            public IntVector2.Rotation Rotation;

            public SectorChoice(Sector sector, IntVector2.Rotation rotation)
            {
                Sector = sector;
                Rotation = rotation;
            }
        }

        private SectorImporter m_sectorData;

        public SectorAssembler()
        {
            //Load sector data
            m_sectorData = new SectorImporter();
        }

        /// <summary>
        /// Fill the provided map with randomly-chosen, randomly-rotated sectors, guaranteeing an entrance sector, 
        /// an exit sector, and a specified number of objective sectors.
        /// </summary>
        public void FillMap(Map map, int num_objectives)
        {
            IntVector2.Rotation[] rotations = (IntVector2.Rotation[]) Enum.GetValues(typeof(IntVector2.Rotation));

            // Choose sectors to make up the map.
            SectorChoice[,] sector_choices = new SectorChoice[map.Width / Sector.Width, map.Height / Sector.Height];

            // - Build a list of sector slots in the map that need to be filled.
            List<IntVector2> empty_slots = new List<IntVector2>();
            for (int x = 0; x < sector_choices.GetLength(0); ++x)
            {
                for (int y = 0; y < sector_choices.GetLength(1); ++y)
                {
                    empty_slots.Add(new IntVector2(x, y));
                }
            }

            // - Randomise the order of the list, and turn it into a queue.
            empty_slots.Shuffle();
            Queue<IntVector2> slot_queue = new Queue<IntVector2>(empty_slots);

            // - Work through the queue, assigning sectors to all empty slots.
            IntVector2 current_slot;

            // -- Add an entrance and an exit sector.
            current_slot = slot_queue.Dequeue();
            sector_choices[current_slot.x, current_slot.y] = new SectorChoice(
                m_sectorData.getRandomEntranceSector(),
                rotations[UnityEngine.Random.Range(0, rotations.Length)]
            );

            current_slot = slot_queue.Dequeue();
            sector_choices[current_slot.x, current_slot.y] = new SectorChoice(
                m_sectorData.getRandomExitSector(),
                rotations[UnityEngine.Random.Range(0, rotations.Length)]
            );

            // -- Add the required number of objective sectors.
            for (int i = 0; i < num_objectives; ++i)
            {
                current_slot = slot_queue.Dequeue();
                sector_choices[current_slot.x, current_slot.y] = new SectorChoice(
                    m_sectorData.getRandomObjectiveSector(),
                    rotations[UnityEngine.Random.Range(0, rotations.Length)]
                );
            }

            // -- Fill any remaining sector slots with random miscellaneous sectors.
            while (slot_queue.Count > 0)
            {
                current_slot = slot_queue.Dequeue();
                sector_choices[current_slot.x, current_slot.y] = new SectorChoice(
                    m_sectorData.getRandomMiscSector(),
                    rotations[UnityEngine.Random.Range(0, rotations.Length)]
                );
            }

            //Write m_sectorData to map.
            for (int x = 0; x < sector_choices.GetLength(0); ++x)
            {
                for (int y = 0; y < sector_choices.GetLength(1); ++y)
                {
                    Sector sector = sector_choices[x, y].Sector;
                    IntVector2.Rotation rotation = sector_choices[x, y].Rotation;
                    sector.WriteToMap(map, new IntVector2(x * Sector.Width, y * Sector.Height), rotation);
                }
            }
        }
    }
}
