using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class FrameThread : Singleton<FrameThread>
{
	private const int MAX_JOB_LIST = 200;

	// Threading variables.
	private static List<Job> Jobs = new List<Job> (MAX_JOB_LIST);
	private static AutoResetEvent resetEvent;
	private static Thread worker;
	private static bool working;

	// Frame logging.
	private static string frameMessage;
	private static bool isFrameLog;

	// Frame variables.
	private float deltaTime;

	void Awake()
	{
		resetEvent = new AutoResetEvent(false);
		working = true;

		worker = new Thread (ThreadUpdate);
		worker.IsBackground = true;
		worker.Start();
	}

	void Destroy()
	{
		working = false;
		resetEvent.Close();
	}

	void Update()
	{
		lock (Jobs) {
			if (Jobs.Count > 0) {
				deltaTime = Time.deltaTime;
				resetEvent.Set ();
			}
		}

		if (isFrameLog) {
			Debug.Log (frameMessage);
			isFrameLog = false;
		}
	}

	void ThreadUpdate()
	{
		while (working)
		{
			resetEvent.WaitOne();

			// To avoid computing new elements
			int JobCount = Jobs.Count;

			for (int i = 0; i < JobCount; ++i)
			{
				Job item = Jobs[i];
				item.callback (item, deltaTime);
			}

			// Removing all jobs (RemoveAll is O(n), and Remove too)
			lock (Jobs) {
				Jobs.RemoveAll(EndWithJobs);
			}
		}
	}

	public void AddJob(Job Job)
	{
		if (Jobs.Count > MAX_JOB_LIST) {
			throw new InvalidOperationException ("Reached the maximum number of jobs.");
		}

		lock (Jobs) {
			Jobs.Add(Job);
		}
	}

	private static bool EndWithJobs(Job job) {
		return job.completed;
	}

	private static void Log(string message)
	{
		frameMessage = message;
		isFrameLog = true;
	}
}