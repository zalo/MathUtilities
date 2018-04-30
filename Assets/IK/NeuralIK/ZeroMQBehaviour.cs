using System.Threading;
using System.Collections.Concurrent;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public abstract class ZeroMQBehaviour : MonoBehaviour {
  private NetMqPair _netMqPair;

  protected abstract void HandleMessage(string message);

  protected virtual void Start() {
    _netMqPair = new NetMqPair(HandleMessage);
    _netMqPair.Start();
  }

  protected virtual void Update() {
    _netMqPair.Update();
  }

  protected virtual void OnDestroy() {
    _netMqPair.Stop();
  }

  protected virtual void Send(string message) {
    _netMqPair.Send(message);
  }
}

public class NetMqPair {
  public delegate void MessageDelegate(string message);
  private readonly MessageDelegate _messageDelegate;
  private readonly ConcurrentQueue<string> _incomingMessageQueue = new ConcurrentQueue<string>();
  private readonly ConcurrentQueue<string> _outgoingMessageQueue = new ConcurrentQueue<string>();

  private readonly Thread _connectionWorker;
  private bool _connectionCancelled;
  private bool _createConnection;

  private void ConnectionWork() {
    AsyncIO.ForceDotNet.Force();
    using (var pairSocket = new PairSocket()) {
      pairSocket.Options.ReceiveHighWatermark = 1000;
      if (_createConnection) {
        pairSocket.Bind("tcp://*:12345");
      } else {
        pairSocket.Connect("tcp://localhost:12345");
      }
      //Do one more loop in-case we send out a closing msg and then cancel the connection
      bool flushedBuffer = true; 
      while (!_connectionCancelled || !flushedBuffer) {
        string frameString;
        while (pairSocket.TryReceiveFrameString(out frameString)) {
          _incomingMessageQueue.Enqueue(frameString);
        }
        while (_outgoingMessageQueue.TryDequeue(out frameString)) {
          pairSocket.SendFrame(frameString);
        }
        flushedBuffer = _connectionCancelled;
      }
      pairSocket.Close();
    }
    NetMQConfig.Cleanup();
  }

  public void Send(string message) {
    _outgoingMessageQueue.Enqueue(message);
  }

  public void Update() {
    while (!_incomingMessageQueue.IsEmpty) {
      string message;
      if (_incomingMessageQueue.TryDequeue(out message)) {
        _messageDelegate(message);
      } else {
        break;
      }
    }
  }

  public NetMqPair(MessageDelegate messageDelegate, bool createConnection = false) {
    _createConnection = createConnection;
    _messageDelegate = messageDelegate;
    _connectionWorker = new Thread(ConnectionWork);
  }

  public void Start() {
    _connectionCancelled = false;
    _connectionWorker.Start();
  }

  public void Stop() {
    _connectionCancelled = true;
    _connectionWorker.Join();
  }
}
