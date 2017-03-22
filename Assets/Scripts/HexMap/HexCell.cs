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
                    terrainTypeIndex = 3;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                default:
                    terrainTypeIndex = 4;
                    break;
            }
            RefreshPosition();
            ValidateRivers();

            for (int i = 0; i < roads.Length; i++) {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1) {
                    SetRoad(i, false);
                }
            }

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
            ValidateRivers();
            Refresh();
        }
    }

    public bool IsUnderwater {
        get {
            return waterLevel > elevation;
        }
    }

    public bool HasIncomingRiver {
        get {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver {
        get {
            return hasOutgoingRiver;
        }
    }

    public bool HasRiver {
        get {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd {
        get {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public HexDirection RiverBeginOrEndDirection {
        get {
            return hasIncomingRiver ? incomingRiver : outgoingRiver;
        }
    }

    public bool HasRoads {
        get {
            for (int i = 0; i < roads.Length; i++) {
                if (roads[i]) {
                    return true;
                }
            }
            return false;
        }
    }

    public HexDirection IncomingRiver {
        get {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver {
        get {
            return outgoingRiver;
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

    public float RiverSurfaceY {
        get {
            return
                (elevation + HexMetrics.waterElevationOffset) *
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
            if (specialIndex != value && !HasRiver && !HasTree) {
                specialIndex = value;
                RemoveRoads();
                RefreshSelfOnly();
            }
        }
    }

    public int TreeIndex {
        get {
            return treeIndex;
        }
        set {
            if (treeIndex != value && !HasRiver && !IsSpecial) {
                treeIndex = value;
                RemoveRoads();
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

    public bool Walled {
        get {
            return walled;
        }
        set {
            if (walled != value) {
                walled = value;
                Refresh();
            }
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

    bool walled;

    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;

    [SerializeField]
    HexCell[] neighbors;

    [SerializeField]
    bool[] roads;

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

    public bool HasRiverThroughEdge(HexDirection direction) {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }

    public void RemoveIncomingRiver() {
        if (!hasIncomingRiver) {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveOutgoingRiver() {
        if (!hasOutgoingRiver) {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver() {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction) {
        if (hasOutgoingRiver && outgoingRiver == direction) {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!IsValidRiverDestination(neighbor)) {
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction) {
            RemoveIncomingRiver();
        }
        hasOutgoingRiver = true;
        outgoingRiver = direction;
        specialIndex = 0;

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        neighbor.specialIndex = 0;

        SetRoad((int)direction, false);
    }

    public bool HasRoadThroughEdge(HexDirection direction) {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction) {
        if (
            !roads[(int)direction] && !HasRiverThroughEdge(direction) &&
            !IsSpecial && !GetNeighbor(direction).IsSpecial &&
            GetElevationDifference(direction) <= 1
        ) {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads() {
        for (int i = 0; i < neighbors.Length; i++) {
            if (roads[i]) {
                SetRoad(i, false);
            }
        }
    }

    public int GetElevationDifference(HexDirection direction) {
        int difference = elevation - GetNeighbor(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    bool IsValidRiverDestination(HexCell neighbor) {
        return neighbor && (
            elevation >= neighbor.elevation || waterLevel == neighbor.elevation
        );
    }

    void ValidateRivers() {
        if (
            hasOutgoingRiver &&
            !IsValidRiverDestination(GetNeighbor(outgoingRiver))
        ) {
            RemoveOutgoingRiver();
        }
        if (
            hasIncomingRiver &&
            !GetNeighbor(incomingRiver).IsValidRiverDestination(this)
        ) {
            RemoveIncomingRiver();
        }
    }

    void SetRoad(int index, bool state) {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
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
        writer.Write(walled);

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

        int roadFlags = 0;
        for (int i = 0; i < roads.Length; i++) {
            if (roads[i]) {
                roadFlags |= 1 << i;
            }
        }
        writer.Write((byte)roadFlags);
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
        walled = reader.ReadBoolean();

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

        int roadFlags = reader.ReadByte();
        for (int i = 0; i < roads.Length; i++) {
            roads[i] = (roadFlags & (1 << i)) != 0;
        }
    }

    public void SetCell(int terrainTypeIdx, int eleva) {
        terrainTypeIndex = terrainTypeIdx;
        elevation = eleva;
        RefreshPosition();
        //waterLevel = reader.ReadByte();
        //urbanLevel = reader.ReadByte();
        //farmLevel = reader.ReadByte();
        //plantLevel = reader.ReadByte();
        //specialIndex = reader.ReadByte();
        //walled = reader.ReadBoolean();

        //byte riverData = reader.ReadByte();
        //if (riverData >= 128) {
        //    hasIncomingRiver = true;
        //    incomingRiver = (HexDirection)(riverData - 128);
        //} else {
        //    hasIncomingRiver = false;
        //}

        //riverData = reader.ReadByte();
        //if (riverData >= 128) {
        //    hasOutgoingRiver = true;
        //    outgoingRiver = (HexDirection)(riverData - 128);
        //} else {
        //    hasOutgoingRiver = false;
        //}

        //int roadFlags = reader.ReadByte();
        //for (int i = 0; i < roads.Length; i++) {
        //    roads[i] = (roadFlags & (1 << i)) != 0;
        //}
    }

}