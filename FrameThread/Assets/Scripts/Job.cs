using System;

public class Job
{
	public delegate void Callback(Job job, float deltaTime);
	public Callback callback;
	public bool completed;

	public Job(Callback callBack)
	{
		this.callback = callBack;
		completed = false;
	}
}