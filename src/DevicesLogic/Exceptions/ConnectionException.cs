namespace task7;

class ConnectionException : Exception
{
    public ConnectionException() : base("Wrong netowrk name.") { }
}