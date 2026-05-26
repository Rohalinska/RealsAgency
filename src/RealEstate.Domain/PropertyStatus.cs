namespace RealEstate.Domain;

/// <summary>
/// Енум для представлення статусу нерухомості.
/// </summary>
public enum PropertyStatus
{
    /// <summary> Об'єкт доступний для продажу. </summary>
    Available = 0,
    
    /// <summary> Об'єкт проданий. </summary>
    Sold = 1
}
