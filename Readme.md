This is a simple actionfilter that can instrument your MVC core actions.


## Getting started

Register an [IStastd](https://github.com/TryStatsN/StatsN) in your DI container. 

Now just add the attribute to an action to instrument

```csharp
[instrument("HomePage.Index.ActionTime")]
public IActionResult Index()
{

}
```