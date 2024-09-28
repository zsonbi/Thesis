using Game.World;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    namespace GameTests
    {
        public class RoadGeneratorTests
        {
            [Test]
            public void TestGenerationWhenNoStartGiven()
            {
                RoadGenerator roadGenerator = new RoadGenerator(GameConfig.CHUNK_SIZE, new List<EdgeRoadContainer>(), new Chunk[3, 3]);

                Assert.IsTrue(roadGenerator.EdgeRoads.Count > 0);

                Assert.AreEqual(GameConfig.CHUNK_SIZE / 2, roadGenerator.EdgeRoads[0].EdgeRoad.x);
                Assert.AreEqual(0, roadGenerator.EdgeRoads[0].EdgeRoad.y);
            }

            [Test]
            public void TestGenerationWhenStartWasGiven()
            {
                List<EdgeRoadContainer> edgeRoadContainers = new List<EdgeRoadContainer>();
                edgeRoadContainers.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE / 2, 0), 1, new Vector2Int(0, -1)));
                RoadGenerator roadGenerator = new RoadGenerator(GameConfig.CHUNK_SIZE, edgeRoadContainers, new Chunk[3, 3]);

                Assert.IsTrue(roadGenerator.EdgeRoads.Count > 0);

                Assert.AreEqual(GameConfig.CHUNK_SIZE / 2, roadGenerator.EdgeRoads[0].EdgeRoad.x);
                Assert.AreEqual(GameConfig.CHUNK_SIZE - 1, roadGenerator.EdgeRoads[0].EdgeRoad.y);
            }

            [Test]
            public void TestGenerationWhenMultipleStartWasGiven()
            {
                List<EdgeRoadContainer> edgeRoadContainers = new List<EdgeRoadContainer>();
                edgeRoadContainers.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE / 2, 0), 1, new Vector2Int(0, -1)));
                edgeRoadContainers.Add(new EdgeRoadContainer(new Vector2Int(0, 5), 1, new Vector2Int(-1, 0)));
                edgeRoadContainers.Add(new EdgeRoadContainer(new Vector2Int(GameConfig.CHUNK_SIZE - 1, 5), 1, new Vector2Int(1, 0)));
                RoadGenerator roadGenerator = new RoadGenerator(GameConfig.CHUNK_SIZE, edgeRoadContainers, new Chunk[3, 3]);

                Assert.IsTrue(roadGenerator.EdgeRoads.Count > 2);

                Assert.AreEqual(GameConfig.CHUNK_SIZE / 2, roadGenerator.EdgeRoads[0].EdgeRoad.x);
                Assert.AreEqual(GameConfig.CHUNK_SIZE - 1, roadGenerator.EdgeRoads[0].EdgeRoad.y);
                Assert.IsTrue(roadGenerator.EdgeRoads.Exists(x => x.EdgeRoad.x == 0 && x.EdgeRoad.y == 5));
                Assert.IsTrue(roadGenerator.EdgeRoads.Exists(x => x.EdgeRoad.x == GameConfig.CHUNK_SIZE - 1 && x.EdgeRoad.y == 5));
            }
        }
    }
}