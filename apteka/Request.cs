public class Request
{
    public int ID { get; set; }
    public string Department { get; set; }
    public string MedicineName { get; set; }
    public int Quantity { get; set; }
    public int WarehouseID { get; set; }
    public bool IsClosed { get; private set; } // Статус заявки
    public int QuantityMoved { get; set; } // Количество перемещенных единиц

    public Request(int id, string department, string medicineName, int quantity, int warehouseID)
    {
        ID = id;
        Department = department;
        MedicineName = medicineName;
        Quantity = quantity;
        WarehouseID = warehouseID;
        IsClosed = false; // Изначально заявка открыта
        QuantityMoved = 0; // Изначально перемещенное количество равно 0
    }

    // Метод для закрытия заявки
    public void Close()
    {
        IsClosed = true; // Закрываем заявку
    }
}
