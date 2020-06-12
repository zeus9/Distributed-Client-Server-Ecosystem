using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project2
{
    //  Communication buffer
    public class MultiCellBuffer
    {
        public List<string> buffer;
        public int n;    // buffer size
        private Semaphore getPool, setPool;

        public MultiCellBuffer(int n)
        {
            //SessionToken = token;
            buffer = new List<string>();
            getPool = new Semaphore(0, n);
            setPool = new Semaphore(n, n);
        }

        public bool isEmpty()
        {
            lock (this)
            {
                if (buffer.Count == 0)
                    return true;
                return false;
            }
            
        }


        public void setOneCell(string data) //  adds a cell in buffer
        {
            //bufferPool.Release();
            lock (this)
            {
                setPool.WaitOne();   //  requests a cell resource in the buffer to set
                
                buffer.Add(data);
                
                getPool.Release();
            }
            
        }

        public string getOneCell()  // removes a cell in buffer
        {
            string data;
            lock (this)
            {
                getPool.WaitOne();   //  requests a cell resource in the buffer

                data = buffer[0].ToString();
                buffer.RemoveAt(0);

                setPool.Release();
            }

            return data;
            
        }
    }
}
