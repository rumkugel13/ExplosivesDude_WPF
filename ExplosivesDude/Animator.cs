namespace ExplosivesDude
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class Animator
    {
        private int finalX, finalY, prevX, prevY;

        public Animator()
        {
            this.IsRunning = false;
        }

        public event EventHandler<OnAnimationCompletedEventArgs> AnimationCompleted;

        public bool IsRunning { get; private set; }

        public void MoveAnimation(IAnimatable animatable, int blockSize, int oldX, int oldY, int newX, int newY, int duration)
        {
            this.prevX = oldX;
            this.prevY = oldY;
            this.finalX = newX;
            this.finalY = newY;
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.Completed += this.Animation_Completed;
            animation.From = new Thickness(oldX * blockSize, oldY * blockSize, 0, 0);
            animation.To = new Thickness(newX * blockSize, newY * blockSize, 0, 0);
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            animation.FillBehavior = FillBehavior.HoldEnd;
            animatable.BeginAnimation(FrameworkElement.MarginProperty, animation);
            ////animatable.BeginAnimation(FrameworkElement.MarginProperty, animation, HandoffBehavior.Compose);
            this.IsRunning = true;
        }

        protected virtual void OnAnimationComplete()
        {
            this.AnimationCompleted?.Invoke(this, new OnAnimationCompletedEventArgs(this.prevX, this.prevY, this.finalX, this.finalY));
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            this.IsRunning = false;
            this.OnAnimationComplete();
        }
    }
}
