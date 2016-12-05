var N = 16384;
var ITERATIONS = ITERATIONS = 16384 * 1000;

function GetRandom()
{	
	//console.log("N, ITERATIONS: " + N + ", " + ITERATIONS + ".");

	// Normal Randoms
	var array = Array.apply(null, Array(N)).map(function (x, i) { return 0; })
	
	for (var i = 0; i < ITERATIONS; i++)
	{ 
		var index = Math.floor((Math.random() * N) + 1);
		array[index] += 1;
	}

	var mean = Mean(array, N);
	var deviation = Deviation(array, mean, N);
	//console.log("Mean: " + mean + ", deviation: " + deviation);
	
	//WriteOutput(array, "test.txt");	
	return deviation;
}

function GetRandomCrypto()
{	
	//console.log("N, ITERATIONS: " + N + ", " + ITERATIONS + ".");

	var MASK = 63;
	var array = Array.apply(null, Array(N)).map(function (x, i) { return 0; })
	var cryptoBytes = crypto.randomBytes(ITERATIONS*2);

	for (var i = 0; i < cryptoBytes.length-1; i = i+2)
	{ 
		var index = ((cryptoBytes[i] & MASK) << 8) | cryptoBytes[i+1];
		array[index] += 1;
	}

	var mean = Mean(array, N);
	var deviation = Deviation(array, mean, N);
	//console.log("Mean: " + mean + ", deviation: " + deviation);

	//WriteOutput(array, "testCrypto.txt");
	return deviation;
}

function WriteOutput(data, filename)
{
	var fs = require("fs");
	var path = "C:\\" + filename;
	fs.writeFile(path, data, function(error) {
     		if (error) {
       			console.error("write error:  " + error.message);
     		} else {
      			console.log("Successful Write to " + path);
 		}
	});
}

function Deviation(array, mean, n)
{
	var deviation = 0;
	for (var i = 0; i < n; i++)
	{
		deviation += Math.pow(array[i] - mean, 2); 
	}
	deviation = Math.sqrt(deviation / (n - 1));
	return deviation;
}

function Mean(array, n)
{
	var mean = 0;
	for (var i = 0; i < n; i++)
	{
		mean += array[i]; 
	}	
	mean = mean / n;
	return mean;
}

function RandomAlgorithmsDeviationMean()
{
	var num = 100;
	var mean = 0;
	for (var i = 0; i < num; ++i)
	{
		mean = mean + GetRandom(); 
	}	
	mean = mean / num;
	console.log("Normal deviation mean: " + mean + ", iterations: " + num);

	mean = 0;
	for (i = 0; i < num; i++)
	{
		mean += GetRandomCrypto(); 
	}	
	mean = mean / num;
	console.log("Crypto deviation mean: " + mean + ", iterations: " + num);
}

RandomAlgorithmsDeviationMean();

