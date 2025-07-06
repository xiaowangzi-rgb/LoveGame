
public class IWindowParam
{
    public IWindowParam()
    {
    }
    public IWindowParam(int eventID)
    {
        this.eventID = eventID;
    }
    public int eventID { get; private set; } = 0;
    public virtual object GetParam(int index)
    {
        return null;
    }
}

public class WindowParam<T> : IWindowParam
{
    public WindowParam(T t)
    {
        this.t0 = t;
    }
    public WindowParam(int eventID, T t) : base(eventID)
    {
        this.t0 = t;
    }
    public T t0 { get; private set; }

    public override object GetParam(int index)
    {
        if (index == 0)
        {
            return t0;
        }
        return base.GetParam(index);
    }
}

public class WindowParam<T0, T1> : WindowParam<T0>
{
    public WindowParam(T0 t0, T1 t1) : base(t0)
    {
        this.t1 = t1;
    }
    public WindowParam(int eventID, T0 t0, T1 t1) : base(eventID, t0)
    {
        this.t1 = t1;
    }
    public T1 t1 { get; private set; }

    public override object GetParam(int index)
    {
        if (index == 1)
        {
            return t1;
        }
        return base.GetParam(index);
    }
}

public class WindowParam<T0, T1, T2> : WindowParam<T0, T1>
{
    public WindowParam(T0 t0, T1 t1, T2 t2) : base(t0, t1)
    {
        this.t2 = t2;
    }
    public WindowParam(int eventID, T0 t0, T1 t1, T2 t2) : base(eventID, t0, t1)
    {
        this.t2 = t2;
    }
    public T2 t2 { get; private set; }
    public override object GetParam(int index)
    {
        if (index == 2)
        {
            return t2;
        }
        return base.GetParam(index);
    }
}

public class WindowParam<T0, T1, T2, T3> : WindowParam<T0, T1, T2>
{
    public WindowParam(T0 t0, T1 t1, T2 t2, T3 t3) : base(t0, t1, t2)
    {
        this.t3 = t3;
    }
    public WindowParam(int eventID, T0 t0, T1 t1, T2 t2, T3 t3) : base(eventID, t0, t1, t2)
    {
        this.t3 = t3;
    }
    public T3 t3 { get; private set; }
    public override object GetParam(int index)
    {
        if (index == 3)
        {
            return t3;
        }
        return base.GetParam(index);
    }
}

public class WindowParam<T0, T1, T2, T3, T4> : WindowParam<T0, T1, T2, T3>
{
    public WindowParam(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4) : base(t0, t1, t2, t3)
    {
        this.t4 = t4;
    }
    public WindowParam(int eventID, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4) : base(eventID, t0, t1, t2, t3)
    {
        this.t4 = t4;
    }
    public T4 t4 { get; private set; }
    public override object GetParam(int index)
    {
        if (index == 4)
        {
            return t4;
        }
        return base.GetParam(index);
    }
}

public class WindowParam<T0, T1, T2, T3, T4, T5> : WindowParam<T0, T1, T2, T3, T4>
{

    public WindowParam(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) : base(t0, t1, t2, t3, t4)
    {
        this.t5 = t5;
    }
    public WindowParam(int eventID, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) : base(eventID, t0, t1, t2, t3, t4)
    {
        this.t5 = t5;
    }
    public T5 t5 { get; private set; }
    public override object GetParam(int index)
    {
        if (index == 5)
        {
            return t5;
        }
        return base.GetParam(index);
    }
}

public class WindowParam<T0, T1, T2, T3, T4, T5, T6> : WindowParam<T0, T1, T2, T3, T4, T5>
{

    public WindowParam(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) : base(t0, t1, t2, t3, t4, t5)
    {
        this.t6 = t6;
    }
    public WindowParam(int eventID, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) : base(eventID, t0, t1, t2, t3, t4, t5)
    {
        this.t6 = t6;
    }
    public T6 t6 { get; private set; }
    public override object GetParam(int index)
    {
        if (index == 6)
        {
            return t6;
        }
        return base.GetParam(index);
    }
}
