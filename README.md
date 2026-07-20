# Med.Trading

## Instrument and order relationship

`InstrumentEntity.Symbol` is the business identifier for an instrument. The SQL
configuration creates a unique index on this column, so an instrument symbol can
identify at most one instrument and can safely be used as an alternate key.

`TradeOrderEntity.InstrumentSymbol` is an optional foreign key to
`InstrumentEntity.Symbol`. It is optional by design, rather than being a
required relationship to the instrument's numeric primary key:

- Orders are historical records and must survive instrument catalogue cleanup.
- An instrument that is referenced by an order cannot be deleted. SQL Server
  rejects the delete through the restrictive foreign key instead of clearing
  `TradingOrder.InstrumentSymbol`.
- The `TradingOrder` row and its symbol, side, quantity, price, status, and
  timestamps are therefore retained exactly as stored.
- New orders may also be created without an instrument symbol when the source
  system does not currently have a matching instrument.

This behavior must be retained in future migrations and repository changes.
Do not change the relationship to cascade delete orders or set the symbol to
null: deleting market metadata must never delete or alter trading history.

## Default provider

`InstrumentEntity.DefaultProvider` identifies the provider used as the default
source for that instrument's market data. It is stored as part of the
instrument entity and is required by the current SQL model.

## Instrument caching

`GetTopInstrumentsByVolumeAsync` uses an explicit two-tier, read-only cache
lookup:

- The local in-process cache is checked first and is valid for 10 seconds.
- After local expiry, the distributed cache (`IDistributedCache`, intended to
  be Redis) is checked directly on every request.
- Redis is read-only from this service: it is not populated, refreshed, or
  assigned an expiration here. Redis is either a hit or a miss.
- The repository is called on a Redis miss, a missing Redis registration, or a
  Redis failure.
- Database results are stored only in local memory for 10 seconds.

This intentionally does not use HybridCache for this read because HybridCache
can write through to the distributed cache and repopulate local memory after a
distributed hit. Redis remains externally managed, while the database remains
the source-of-truth fallback.

This cache is intended only for the read-only top-instruments display. It must
not be used as the source of truth for order execution or other trading
decisions.

`GetInstrumentBySymbolAsync` deliberately bypasses `HybridCache` and reads
through the repository every time. This keeps symbol lookups in a clean,
current state and avoids stale instrument data being used for trading
operations.

## Database Migrations

Generate a new migration:
```bash
dotnet ef migrations add InitialCreate --project Med.Trading.Infrastructure.Sql
```

Generate idempotent SQL script (from scratch):
```bash
dotnet ef migrations script 0 InitialCreate --idempotent \
  --output Med.Trading.Infrastructure.Sql/Migrations/migration_script.sql \
  --project Med.Trading.Infrastructure.Sql
```
