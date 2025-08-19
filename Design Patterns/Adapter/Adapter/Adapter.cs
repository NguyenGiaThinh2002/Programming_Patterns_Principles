// Target: giao diện mà client mong muốn
public interface ITwoPinPlug
{
    void ConnectWithTwoPins();
}

// Adaptee: class có sẵn, nhưng dùng 3 chấu
public class ThreePinSocket
{
    public void ConnectWithThreePins()
    {
        Console.WriteLine("Kết nối bằng ổ cắm 3 chấu!");
    }
}

// Adapter: chuyển đổi từ 3 chấu sang 2 chấu
public class SocketAdapter : ITwoPinPlug
{
    private readonly ThreePinSocket _threePinSocket;

    public SocketAdapter(ThreePinSocket threePinSocket)
    {
        _threePinSocket = threePinSocket;
    }

    public void ConnectWithTwoPins()
    {
        Console.WriteLine("Dùng Adapter để chuyển từ 2 chấu sang 3 chấu...");
        _threePinSocket.ConnectWithThreePins();
    }
}

// Client: Laptop cần cắm 2 chấu
class Laptop
{
    private readonly ITwoPinPlug _plug;

    public Laptop(ITwoPinPlug plug)
    {
        _plug = plug;
    }

    public void Charge()
    {
        Console.WriteLine("Laptop cần cắm 2 chấu để sạc...");
        _plug.ConnectWithTwoPins();
    }
}

// Test
class Program
{
    static void Main()
    {
        // Có sẵn ổ cắm 3 chấu
        ThreePinSocket threePinSocket = new ThreePinSocket();

        // Dùng adapter để chuyển từ 3 chấu sang 2 chấu
        ITwoPinPlug adapter = new SocketAdapter(threePinSocket);

        // Laptop chỉ nhận 2 chấu, nhưng nhờ Adapter nó vẫn dùng được
        Laptop laptop = new Laptop(adapter);
        laptop.Charge();

        /*
        Output:
        Laptop cần cắm 2 chấu để sạc...
        Dùng Adapter để chuyển từ 2 chấu sang 3 chấu...
        Kết nối bằng ổ cắm 3 chấu!
        */
    }
}