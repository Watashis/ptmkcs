using ptmkcs;

static string[] ParseCmd(string[] args)
{
    if (!args.Any())
    {
        Console.WriteLine("Command not found");
        Environment.Exit(0);
    }
    string[] result = new string[4];
    result[0] = args[0];
    if (args.Length == 1)
        return result;
    else
    {
        result[3] = args[^1];
        result[2] = args[^2];
        args[0] = "";
        args[^1] = "";
        args[^2] = "";
        result[1] = string.Join(" ", args).Trim();
    }
    return result;
}
static string PrintArray(List<PeopleFull> peoples)
{
    var text = "";
    peoples.ForEach(p =>
    {
        text += $"{p.print()}\n";
    });
    return text;
}

DataBase db = new();
var cmd = ParseCmd(args);
var text = "";
if (cmd[0] == "1")
    db.Create();
if (cmd[0] == "2")
    text = db.AddPeoples(cmd[1], cmd[2], cmd[3]).print();
if (cmd[0] == "3")
    text = PrintArray(db.GetUnique());
if (cmd[0] == "4")
    db.Generate();
if (cmd[0] == "5")
    text = $"Took {db.GetPeoples()} ms.";
if (cmd[0] == "6")
    text = db.Acceleration();
Console.WriteLine(text);