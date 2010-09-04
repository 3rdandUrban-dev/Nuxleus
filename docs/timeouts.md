Timeouts in Manos
=================

Manos allows for tasks to be scheduled using its timeout framework.  These tasks are performed on the main thread, in Manos' main IO loop, so they shouldn't be used for long running operations. Timeouts are mostly useful for small tasks that are time based, like flushing a cache after a certain amount of time.

Timeouts are not guaranteed to start at an exact time, but they will always wait until the specified amount of time has expired before running.


Here is a simple example of a timeout:

    MyManosApp ()
    {
        AddTimeout (TimeSpan.FromMinutes (5), app => app.Cache.Clear ());
    }

The previous example would be executed just once, if we want to clear our cache every five minutes we can use a RepeatBehavior:

    MyManosApp ()
    {
        AddTimeout (TimeSpan.FromMinutes (5), RepeatBehavior.Forever, app => app.Cache.Clear ());
    }

The RepeatBehavior class also allows you to specify the number of times to perform a task using the Iterations methods:

    MyManosApp ()
    {
        AddTimeout (TimeSpan.FromMinutes (5), RepeatBehavior.Iterations (10), app => app.Cache.Clear ());
    }

Timeout Callbacks
-----------------
The timeout callback is a delegate that will pass the MangoApp and the data object it was added with to your handler. If no data object is used when the timeout was added, data will be null.

    MyManosApp ()
    {
        MyData data = new MyData ();
        
        AddTimeout (TimeSpan.FromSeconds (10), FooHandler, data);
    }

    void FooHandler (ManosApp app, object data)
    {
        MyData mydata = data as MyData;
    }


Custom RepeatBehaviors
----------------------

If your application needs a RepeatBehavior more sophisticated than once, forever, or number of iterations you can create a custom RepeatBehavior.  This is done by implementing the IRepeatBehavior interface.  This interface has a two methods ShouldContinueToRepeat, and RepeatPerformed.  The RepeatPerformed notifies the RepeatBehavior that a Repetion has been performed. Manos will then call ShouldContinueToRepeat. If ShouldContinueToRepeat returns false, the timeout is stopped.

Here is a simple example that will clear a cache every five minutes until November 6th 2001.

    class RepeatUntilTime : IRepeatBehavior {

        public DateTime Time {
            get;
            set;
        }

        public bool ShouldContinueToRepeat (ManosApp app)
        {
            return DateTime.Now > Time;
        }

        public void RepeatPerformed ()
        {
        }
    }

    class MyManosApp : ManosApp {

        public ManosApp ()
        {
            AddTimeout (TimeSpan.FromMinutes (5), new RepeatUntilTime (2001, 11, 6), app => app.Cache.Clear ());
        }
    }

