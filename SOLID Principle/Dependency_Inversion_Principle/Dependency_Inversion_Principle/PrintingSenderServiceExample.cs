using System;
using System.Collections.Concurrent;

public class SenderService
{
    static void Main(string[] args)
    {
        var queue = new BlockingCollection<PrintingDataEntry>();
        var storage = new InMemoryStorageService();

        // Composite sender: API + File + SAP
        IDataSender sender = new MultiDataSender(
            new ApiDataSender(),
            new FileDataSender(),
            new SapDataSender()
        );

        var service = new PrintingSenderService(queue, storage, sender, "http://example.com/api/print");
        service.Start();

        queue.Add(new PrintingDataEntry
        {
            Code = "QR123",
            UniqueCode = "UC123",
            PrintedDate = DateTime.Now.ToString("s")
        });

        queue.CompleteAdding();
        Console.ReadLine();
    }

    // Step 1: Define the abstraction
    public interface IDataSender
    {
        Task<ResponsePrinted> SendAsync(string endpoint, object data);
    }
    // Step 2: Composite sender implementation
    public class MultiDataSender : IDataSender
    {
        private readonly List<IDataSender> _senders;

        public MultiDataSender(params IDataSender[] senders)
        {
            _senders = senders.ToList();
        }

        public async Task<ResponsePrinted> SendAsync(string endpoint, object data)
        {
            ResponsePrinted finalResponse = new ResponsePrinted();
            foreach (var sender in _senders)
            {
                var response = await sender.SendAsync(endpoint, data);

                // Merge responses (basic example)
                finalResponse.is_success |= response.is_success;
                finalResponse.is_success_sap |= response.is_success_sap;
                finalResponse.message += $" | {response.message}";
                finalResponse.message_sap += $" | {response.message_sap}";
            }
            return finalResponse;
        }
    }

    // Step 3: Keep old senders (API + File + maybe SAP)

    public class ApiDataSender : IDataSender
    {
        public async Task<ResponsePrinted> SendAsync(string endpoint, object data)
        {
            await Task.Delay(200);
            Console.WriteLine($"[ApiDataSender] Sending to API at {endpoint}");
            return new ResponsePrinted
            {
                is_success = true,
                is_success_sap = true,
                message = "API accepted",
                message_sap = "SAP synced"
            };
        }
    }

    public class FileDataSender : IDataSender
    {
        public async Task<ResponsePrinted> SendAsync(string endpoint, object data)
        {
            await Task.Delay(100);
            Console.WriteLine($"[FileDataSender] Writing to file at {endpoint}");
            return new ResponsePrinted
            {
                is_success = true,
                is_success_sap = false,
                message = "Written to file",
                message_sap = "Not applicable"
            };
        }
    }

    public class SapDataSender : IDataSender
    {
        public async Task<ResponsePrinted> SendAsync(string endpoint, object data)
        {
            await Task.Delay(150);
            Console.WriteLine($"[SapDataSender] Sending to SAP system at {endpoint}");
            return new ResponsePrinted
            {
                is_success = false,
                is_success_sap = true,
                message = "Not SaaS, SAP only",
                message_sap = "SAP processed"
            };
        }
    }

    // Step 4: Service (unchanged!)
    public class PrintingSenderService
    {
        private readonly BlockingCollection<PrintingDataEntry> _queue;
        private readonly IStorageService<PrintingDataEntry> _storageService;
        private readonly IDataSender _dataSender;
        private readonly string _endpoint;

        public PrintingSenderService(
            BlockingCollection<PrintingDataEntry> queue,
            IStorageService<PrintingDataEntry> storageService,
            IDataSender dataSender,
            string endpoint)
        {
            _queue = queue;
            _storageService = storageService;
            _dataSender = dataSender;
            _endpoint = endpoint;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                foreach (var entry in _queue.GetConsumingEnumerable())
                {
                    await ProcessEntryAsync(entry);
                }
            });
        }

        private async Task ProcessEntryAsync(PrintingDataEntry entry)
        {
            try
            {
                object printedContent = new RequestPrinted
                {
                    id = entry.Id,
                    qr_code = entry.Code,
                    unique_code = entry.UniqueCode,
                    printed_date = DateTime.Parse(entry.PrintedDate),
                    resource_code = "LINE01",
                    resource_name = "Line A"
                };

                var response = await _dataSender.SendAsync(_endpoint, printedContent);

                if (response.is_success || response.is_success_sap)
                {
                    _storageService.MarkAsSent(entry.Id, entry.PrintedDate,
                        response.is_success ? "success" : "failed",
                        response.is_success_sap ? "success" : "failed",
                        response.message, response.message_sap);
                }
                else
                {
                    _storageService.MarkAsFailed(entry.Id, entry.PrintedDate,
                        "failed", "failed",
                        response.message, response.message_sap);
                    _queue.Add(entry); // retry
                }
            }
            catch (Exception ex)
            {
                _storageService.MarkAsFailed(entry.Id, entry.PrintedDate,
                    "failed", "failed", ex.Message, string.Empty);
                _queue.Add(entry);
            }
        }

        public void Stop() => _queue.CompleteAdding();
    }
    // Step 5: Storage Service
    public class InMemoryStorageService : IStorageService<PrintingDataEntry>
    {
        public void MarkAsSent(Guid id, string printedDate, string saasStatus, string sapStatus, string saasError, string sapError)
        {
            Console.WriteLine($"[Storage] Entry {id} marked as SENT. SaaS: {saasStatus}, SAP: {sapStatus}");
        }

        public void MarkAsFailed(Guid id, string printedDate, string saasStatus, string sapStatus, string saasError, string sapError)
        {
            Console.WriteLine($"[Storage] Entry {id} marked as FAILED. Error: {saasError}");
        }
    }

    // Step 6: Program

  
}

public interface IStorageService<T>
{
    void MarkAsSent(Guid id, string printedDate, string saasStatus, string sapStatus, string saasError, string sapError);
    void MarkAsFailed(Guid id, string printedDate, string saasStatus, string sapStatus, string saasError, string sapError);
}

public class PrintingDataEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; }
    public string UniqueCode { get; set; }
    public string PrintedDate { get; set; }
    public string SaasStatus { get; set; }
    public string SAPStatus { get; set; }
    public string SaasError { get; set; }
    public string SAPError { get; set; }
}

public class RequestPrinted
{
    public Guid id { get; set; }
    public string qr_code { get; set; }
    public string unique_code { get; set; }
    public DateTime printed_date { get; set; }
    public string resource_code { get; set; }
    public string resource_name { get; set; }
}

public class ResponsePrinted
{
    public bool is_success { get; set; }
    public bool is_success_sap { get; set; }
    public string message { get; set; }
    public string message_sap { get; set; }
}
