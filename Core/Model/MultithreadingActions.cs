using Core.Model.Type;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Core.Model
{
    public class MultithreadingActions
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public MultithreadingActions()
        {
            
        } 

        /// <summary>
        /// Play all save work
        /// </summary>
        public void Play()
        {
            Complete.isPaused = false;
            Differential.isPaused = false;
        }

        /// <summary>
        /// Play one save work
        /// </summary>
        /// <param name="name">Save work name</param>
        public void Play(string name)
        {
            Debug.WriteLine("Play " + name);
        }

        /// <summary>
        /// Pause all save work
        /// </summary>
        public void Pause()
        {
            Complete.isPaused = true;
            Differential.isPaused = true;
        }

        /// <summary>
        /// Pause one save work
        /// </summary>
        /// <param name="name">Save work name</param>
        public void Pause(string name)
        {
            Debug.WriteLine("Pause " + name);
        }

        /// <summary>
        /// Stop all save work
        /// </summary>
        public void Stop()
        {
            Complete.isStop = true;
            Differential.isStop = true;
        }

        /// <summary>
        /// Stop one save work
        /// </summary>
        /// <param name="name">Save work name</param>
        public void Stop(string name)
        {
            Debug.WriteLine("Stop " + name);
        }
    }
}
