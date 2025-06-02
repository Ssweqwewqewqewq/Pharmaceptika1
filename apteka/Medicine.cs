using System;

public class Medicine
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Symptoms { get; set; }
    public bool IsPrescription { get; set; }
    public string ActiveIngredient { get; set; }
    public string Contraindications { get; set; }
    public string SideEffects { get; set; }
    public float Price { get; set; } // Изменено на float

    public Medicine(int id, string name, int quantity, DateTime expirationDate, string symptoms, bool isPrescription, string activeIngredient, string contraindications, string sideEffects, float price) // Изменено на float
    {
        ID = id;
        Name = name;
        Quantity = quantity;
        ExpirationDate = expirationDate;
        Symptoms = symptoms;
        IsPrescription = isPrescription;
        ActiveIngredient = activeIngredient;
        Contraindications = contraindications;
        SideEffects = sideEffects;
        Price = price; // Инициализация цены
    }
}
