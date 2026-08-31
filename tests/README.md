# WCS route migration checks

The public WCS HTTP prefix is `/wcs`; the old `/ecs` prefix is not retained as an alias.
`/hub` remains the SignalR endpoint. WMS public `/StockTasks` and `/WcsTasks` routes are unchanged.

Run from the repository root:

```powershell
dotnet run --project tests/Wcs.RouteContracts/Wcs.RouteContracts.csproj
dotnet run --project tests/Wms.RequestContracts/Wms.RequestContracts.csproj
npm --prefix RenXing_WCS_Web run test:routes
```

- `Wcs.RouteContracts` reads compiled controller/action route attributes and checks the public methods consumed by clients.
- `Wms.RequestContracts` executes the real `WcsApiManager` and HTTP serialization with an in-memory recording transport.
- Frontend tests execute Vue request-building methods and actual Axios/SignalR URL construction. Only UI imports and network I/O are substituted.

These checks do not start hosted jobs or contact MySQL, Redis, PLCs, or physical equipment.
They verify route/request contracts, not order completion or inventory correctness.

## Local frontend configuration

- WCS monitor defaults to `http://127.0.0.1:5200`; `VUE_APP_WCS_API_URL` overrides it at build time for both HTTP and SignalR.
- WMS frontend dev/test/production builds currently target `http://127.0.0.1:5100` via `VITE_API_URL`.
- Loopback targets are for same-machine testing. Remote browser deployments must provide the appropriate service origins and CORS settings.
- WMS's preexisting `/chatHub` setting remains inactive; changing its origin does not add backend Hub support.

Known separate integration work: WCS-to-WMS login/callback contracts, batch check-order payloads, and the unsupported WMS `PlcNode` operation. Do not infer full business interoperability from these route checks.
