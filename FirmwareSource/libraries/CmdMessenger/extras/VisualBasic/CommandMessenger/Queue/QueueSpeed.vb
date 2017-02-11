#Region "CmdMessenger - MIT - (c) 2013 Thijs Elenbaas."
'
'  CmdMessenger - library that provides command based messaging
'
'  Permission is hereby granted, free of charge, to any person obtaining
'  a copy of this software and associated documentation files (the
'  "Software"), to deal in the Software without restriction, including
'  without limitation the rights to use, copy, modify, merge, publish,
'  distribute, sublicense, and/or sell copies of the Software, and to
'  permit persons to whom the Software is furnished to do so, subject to
'  the following conditions:
'
'  The above copyright notice and this permission notice shall be
'  included in all copies or substantial portions of the Software.
'
'  Copyright 2013 - Thijs Elenbaas
'
#End Region


Imports Microsoft.VisualBasic
Imports System
Imports System.Threading

Namespace CommandMessenger
	''' <summary> Class that regulates sleeping  within a queue thread.  
	''' 		  Based on load the sleep time will increase or decrease </summary>
	Public Class QueueSpeed
		Private _queueCount As Long
		Private _prevTime As Long
		Private _sleepTime As Double
		Private Const Alpha As Double = 0.8
		Private ReadOnly _targetQueue As Double = 0.5
		Private ReadOnly _maxSleep As Long = 50
		Private Const MinSleep As Long = 0

		''' <summary> Gets or sets the QueueSpeed name. Used for debugging </summary>
		''' <value> The object name. </value>
		Private privateName As String
		Public Property Name() As String
			Get
				Return privateName
			End Get
			Set(ByVal value As String)
				privateName = value
			End Set
		End Property

		''' <summary> Initialize the queue speed with a target filling of the queue. </summary>
		''' <param name="targetQueue"> target filling of the queue. </param>
		Public Sub New(ByVal targetQueue As Double)
			_targetQueue = targetQueue
			_prevTime = TimeUtils.Millis
			_sleepTime = 0
		End Sub

		''' <summary> Initialize the queue speed with a target filling of the queue. </summary>
		''' <param name="targetQueue"> target filling of the queue. </param>
		''' <param name="maxSleep">Maximum sleep times</param>
		Public Sub New(ByVal targetQueue As Double, ByVal maxSleep As Long)
			_targetQueue = targetQueue
			_prevTime = TimeUtils.Millis
			_sleepTime = 0
			_maxSleep = maxSleep
		End Sub

		''' <summary> Calculates the sleep time taking into account work being done in queue. </summary>
		Public Sub CalcSleepTime()
			Dim currentTime = TimeUtils.Millis
			Dim deltaT = Math.Max((currentTime-_prevTime),1)
			Dim processT = deltaT- _sleepTime
			Dim rate As Double = CDbl(_queueCount) / CDbl(deltaT)
			Dim targetT As Double = Math.Min(_targetQueue / rate, _maxSleep)
			Dim compensatedT As Double = Math.Min(Math.Max(targetT - processT, 0), 1e6)
			_sleepTime = Math.Max(CDbl(Math.Min((Alpha * _sleepTime + (1 - Alpha) * compensatedT), CDbl(_maxSleep))), MinSleep)

			'if (Name != "" && Name != null)
			'{
			'     Console.WriteLine("Rate {1} {0}", Name, rate);
			'    Console.WriteLine("Sleeptime {1} {0}", Name, _sleepTime);
			'}

			' Reset
			_prevTime = currentTime
			_queueCount = 0
		End Sub

		''' <summary> Calculates the sleep without time taking into account work being done in queue. </summary>
		Public Sub CalcSleepTimeWithoutLoad()
			Dim currentTime = TimeUtils.Millis
			Dim deltaT = Math.Max((currentTime - _prevTime), 1)
			Dim rate As Double = _queueCount / CDbl(deltaT)
			Dim targetT As Double = Math.Min(_targetQueue / rate,_maxSleep)
			_sleepTime = Math.Max((Alpha * _sleepTime + (1 - Alpha) * targetT), MinSleep)
			'if (Name != "" && Name != null)
			'{
				'Console.WriteLine("Rate {1} {0}", Name, rate);
				'Console.WriteLine("targetT {1} {0}", Name, targetT);
				'Console.WriteLine("sleepTime {1} {0}", Name, _sleepTime);
			'}

			' Reset
			_prevTime = currentTime
			_queueCount = 0
		End Sub

		''' <summary> Adds a unit to the load count. </summary>
		Public Sub AddCount()
			_queueCount += 1
		End Sub

		''' <summary> Adds a count units to the load count. </summary>
		''' <param name="count"> Number of load units to increase. </param>
		Public Sub AddCount(ByVal count As Integer)
			_queueCount+= count
		End Sub

		''' <summary> Sets the count units to the load count. </summary>
		''' <param name="count"> Number of load units to increase. </param>
		Public Sub SetCount(ByVal count As Integer)
			_queueCount = count
		End Sub

		''' <summary> Resets the count units to zero. </summary>
		Public Sub ResetCount()
			_queueCount = 0
		End Sub

		''' <summary> Perform the sleep based on load. </summary>
		Public Sub Sleep()
			Sleep(CLng(Fix(_sleepTime)))
		End Sub

		Public Sub Sleep(ByVal millis As Long)
			Thread.Sleep(TimeSpan.FromMilliseconds(millis))
		End Sub
	End Class
End Namespace
