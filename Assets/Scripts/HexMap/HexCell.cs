using UnityEngine;
using System.IO;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;

    public RectTransform uiRect;

    public HexGridChunk chunk;

    public int Elevation {
        get {
            return elevation;
        }
        set {
            if (elevation == value) {
                return;
            }
            elevation = value;

            switch (elevation) {
                case 0:
                    terrainTypeIndex = 0;
                    break;
                case 1:
                    terrainTypeIndex = 1;
                    break;
                case 2:
                case 3:
                case 4:
                    terrainTypeIndex = 2;
                    break;
                case 5:
                case 6:
                    terrainTypeIndex = 3;
                    break;
                case 7:
                case 8:
                case 9:
                default:
                    terrainTypeIndex = 4;
                    break;
            }
            RefreshPosition();
            Refresh();
        }
    }

    public int WaterLevel {
        get {
            return waterLevel;
        }
        set {
            if (waterLevel == value) {
                return;
            }
            waterLevel = value;
            Refresh();
        }
    }

    public bool IsUnderwater {
        get {
            return waterLevel > elevation;
        }
    }

    public Vector3 Position {
        get {
            return transform.localPosition;
        }
    }

    public float StreamBedY {
        get {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    public float WaterSurfaceY {
        get {
            return
                (waterLevel + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    public int RockLevel {
        get {
            return rockLevel;
        }
        set {
            if (rockLevel != value) {
                rockLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int GrassLevel {
        get {
            return grassLevel;
        }
        set {
            if (grassLevel != value) {
                grassLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int AnimalLevel {
        get {
            return animalLevel;
        }
        set {
            if (animalLevel != value) {
                animalLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int SpecialIndex {
        get {
            return specialIndex;
        }
        set {
            if (specialIndex != value && !HasTree) {
                specialIndex = value;
                RefreshSelfOnly();
            }
        }
    }

    public int TreeIndex {
        get {
            return treeIndex;
        }
        set {
            if (treeIndex != value && !IsSpecial) {
                treeIndex = value;
                RefreshSelfOnly();
            }
        }
    }

    public bool IsSpecial {
        get {
            return specialIndex > 0;
        }
    }

    public bool HasTree {
        get {
            return treeIndex > 0;
        }
    }

    public int TerrainTypeIndex {
        get {
            return terrainTypeIndex;
        }
        set {
            if (terrainTypeIndex != value) {
                terrainTypeIndex = value;
                Refresh();
            }
        }
    }

    int terrainTypeIndex;

    int elevation = int.MinValue;
    int waterLevel;

    int rockLevel, grassLevel, animalLevel;

    int specialIndex;

    int treeIndex;

    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction) {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction) {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    public HexEdgeType GetEdgeType(HexCell otherCell) {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }

    public int GetElevationDifference(HexDirection direction) {
        int difference = elevation - GetNeighbor(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    void RefreshPosition() {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        position.y +=
            (HexMetrics.SampleNoise(position).y * 2f - 1f) *
            HexMetrics.elevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    void Refresh() {
        if (chunk) {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++) {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk) {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    void RefreshSelfOnly() {
        chunk.Refresh();
    }

    public void Save(BinaryWriter writer) {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)elevation);
        writer.Write((byte)waterLevel);
        writer.Write((byte)rockLevel);
        writer.Write((byte)grassLevel);
        writer.Write((byte)animalLevel);
        writer.Write((byte)specialIndex);
       

        if (hasIncomingRiver) {
            writer.Write((byte)(incomingRiver + 128));
        } else {
            writer.Write((byte)0);
        }

        if (hasOutgoingRiver) {
            writer.Write((byte)(outgoingRiver + 128));
        } else {
            writer.Write((byte)0);
        }
    }

    public void Load(BinaryReader reader) {
        terrainTypeIndex = reader.ReadByte();
        elevation = reader.ReadByte();
        RefreshPosition();
        waterLevel = reader.ReadByte();
        rockLevel = reader.ReadByte();
        grassLevel = reader.ReadByte();
        animalLevel = reader.ReadByte();
        specialIndex = reader.ReadByte();
       
        byte riverData = reader.ReadByte();
        if (riverData >= 128) {
            hasIncomingRiver = true;
            incomingRiver = (HexDirection)(riverData - 128);
        } else {
            hasIncomingRiver = false;
        }

        riverData = reader.ReadByte();
        if (riverData >= 128) {
            hasOutgoingRiver = true;
            outgoingRiver = (HexDirection)(riverData - 128);
        } else {
            hasOutgoingRiver = false;
        }

    }
}