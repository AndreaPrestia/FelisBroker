
# 🚀 FelisBroker

**FelisBroker** is a lightweight, self-hosted event ingestion engine that collects events from various protocols (HTTP, MQTT, Redis Pub/Sub, etc.), processes them in-memory, and persists them using embedded storage (ZoneTree). Designed for edge and low-infra environments — no cloud infrastructure required.

---

## ✨ Features

- ✅ **Pluggable Sources**: HTTP, MQTT, Redis Pub/Sub, RabbitMQ, and more
- ✅ **Embedded Storage**: Efficient, persistent log-based storage with ZoneTree
- ✅ **Offset Tracking**: Each topic maintains ordered offsets for streaming
- ✅ **Streaming API**: Read events by offset range (`IAsyncEnumerable`)
- ✅ **Authentication**: Supports Basic Auth, JWT, and Mutual TLS (mTLS)
- ✅ **Memory-efficient Indexing**: Fast key-based lookups
- ✅ **Fully Configurable**: Driven by `origins.json` at startup
- ✅ **Minimal Dependencies**: No external databases or brokers required

---

## 🧱 Architecture


---

## 📦 Storage Model

- Events are stored as:  
  `Key: <topic>|<offset>`  
  `Value: JSON payload`

- Partitioned ZoneTrees per topic:  
  `/data/topics/sensor.temp.zone/`

- Offsets per topic tracked in:  
  `/data/offsets.db`

---

## ⚙️ Configuration (origins.json)

```json
{
    "Origins": [
        {
            "AuthConfiguration": {
                "Audience": "clients",
                "AuthScheme": "Jwt",
                "DurationInMinutes": 10,
                "Issuer": "my-app",
                "RequireHttps": true,
                "SigningKey": "super-secret"
            },
            "Destination": "Destination",
            "Name": "HttpJwtSource",
            "OriginType": "Http"
        },
        {
            "Broker": "tcp://broker.hivemq.com",
            "ClientId": "felis-client-1",
            "Name": "MqttSource",
            "OriginType": "Mqtt",
            "Topic": "iot/devices/#"
        },
        {
            "Broker": "tcp://broker.hivemq.com",
            "ClientId": "felis-client-1",
            "Name": "MqttSource",
            "OriginType": "Mqtt",
            "Topic": "iot/devices/#"
        },
        {
            "Destination": "filesystem.logs",
            "IncludeSubdirectories": "true",
            "Name": "LogFileWatcher",
            "OriginType": "FileSystem",
            "Path": "/var/log/myapp/",
            "Pattern": "*.log",
            "ReadNewLinesOnly": "true"
        },
        {
            "Name": "RedisInput",
            "OriginType": "RedisPubSub",
            "Properties": {
                "Channel": "events.*",
                "Host": "localhost",
                "Password": "",
                "Port": "6379"
            },
            "Topic": "events:*"
        }
    ]
}
```

---

## 🔐 Authentication Options (for HTTP data source)

| Type     | Supported | Details                     |
|----------|-----------|-----------------------------|
| Basic    | ✅        | Configurable user/pass      |
| JWT      | ✅        | Validated via `JwtConfig`   |
| mTLS     | ✅        | Thumbprint + subject match  |

---

## 🚀 Usage

### 📁 `Program.cs`

```csharp
builder.Services.AddFelisBroker("data");

var app = builder.Build();

app.MapFelisBrokerEndpoints();

app.Run();
```

---

### 🧪 Example Ingestion (HTTP)

```bash
curl -X POST http://localhost:5000/sensor.temp \
  -H "Authorization: Bearer <jwt>" \
  -d '{"temperature": 24.5, "unit": "C"}'
```

---

### 🔍 Streaming Events by Offset

```csharp
await foreach (var entity in eventStore.ReadMessagesAsync("sensor.temp", 0, 100))
{
    Console.WriteLine($"[{entity.Offset}] {entity.Payload}");
}
```

---

## 🧩 Extending

To add a new source:
1. Create a class inheriting `OriginConfiguration` and `IDataSource`
2. Implement `Validate()`
3. Plug it into `DataSourceFactory`
4. Add config to `origins.json`

---

## 📚 Project Structure

```
/src
  /Configuration
  /Sources
    HttpDataSource.cs
    MqttDataSource.cs
    RedisPubSubDataSource.cs
  /Storage
    EventStore.cs
    OffsetManager.cs
  /Auth
    HttpAuthHelper.cs
  /Hosted
    DataSourceService.cs
```

---

## 🧪 Roadmap

- [ ] Web UI for metrics + ingestion config
- [ ] Dynamic reload of `origins.json`
- [ ] Subscription streaming via HTTP
- [ ] Compression (LZ4) and TTL pruning

---

## 🐾 About the Name

**Felis** is Latin for "cat" — fast, lightweight, independent.
**FelisBroker** is your small, embedded, event-taming feline 🐈.

---

## 🛠️ Requirements

- .NET 8+
- No database required — uses `ZoneTree` (embedded)

---

## 📄 License

MIT — free to use, extend, or integrate.
