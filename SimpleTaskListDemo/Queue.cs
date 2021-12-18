using System;
using System.Collections.Generic;

namespace SimpleTaskListDemo
{
    [Serializable()]
    public class Queue
    {
        private List<QueueNode> nodeList;
        private QueueNode head;

        public Queue()
        {
            nodeList = new List<QueueNode>();
            head = null;
        }
        public Queue(List<QueueNode> _nodeList)
        {
            if (_nodeList.Count > 0)
            {
                nodeList = _nodeList;
                head = nodeList[0];

                for (int i = 0; i < nodeList.Count - 1; i++)
                {
                    if (nodeList[i] != null)
                    {
                        nodeList[i].NextNode = nodeList[i + 1];
                    }
                    else
                    {
                        i = int.MaxValue;
                    }
                }

                nodeList[nodeList.Count - 1].NextNode = null;
            }
            else
            {
                new Queue();
            }
        }
        public List<QueueNode> NodeList
        {
            get => nodeList;
            //set => nodeList = value;
        }
        public QueueNode Head
        {
            get => head;
            set => head = value;
        }
        public bool completeTask()
        {
            // cannot set head to head.next if head is null
            bool isSuccess = (head != null);

            if (isSuccess)
            {
                QueueNode temp = head;
                nodeList.RemoveAt(0);
                //temp = head.NextNode;
                head = temp.NextNode;
            }

            return isSuccess;
        }
    }
    [Serializable()]
    public class QueueNode : IComparable
    {
        private Task data;
        private QueueNode nextNode;

        public QueueNode(Task _data)
        {
            data = _data;
            nextNode = null;
        }

        public Task Data
        {
            get => data;
            //set => data = value;
        }
        public QueueNode NextNode
        {
            get => nextNode;
            set => nextNode = value;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is QueueNode))
            {
                throw new ArgumentException();
            }

            QueueNode temp = (QueueNode)obj;

            return data.ID - temp.data.ID;
        }
    }
}
