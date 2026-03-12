[System.Serializable]
public class VehicleData

/// Definiert die Struktur für einzelne Fahrzeugdatensätze (`VehicleData`).
/// Definiert die Struktur für eine Sammlung von Fahrzeugdatensätzen (`VehicleList`).
///
/// Inputs: Keine dient als Datenmodell.
///
/// Outputs: Keine.
{
    public string FIN;
    public string ABEs;
    public string defReport;
}

[System.Serializable]
public class VehicleList
{
    public VehicleData[] vehicles;
}
