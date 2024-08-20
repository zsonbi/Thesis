using DataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField]
    private BuildingType buildingType;

    [SerializeField]
    private int rowCount;

    [SerializeField]
    private int columnCount;

    [SerializeField]
    private Direction direction = Direction.Down;

    [SerializeField]
    private string addressableKey;

    public BuildingType BuildingType => buildingType;
    public int RowCount => rowCount;
    public int ColumnCount => columnCount;
    public Direction Direction => direction;
    public string AddressableKey => addressableKey;

    public void SetAddressableKey(string newKey)
    {
        this.addressableKey = newKey;
    }
}