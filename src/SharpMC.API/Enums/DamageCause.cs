using System.ComponentModel;

namespace SharpMC.API.Enums
{
    public enum DamageCause
    {
        [Description("{0} went MIA")] Unknown,
        [Description("{0} was pricked to death")] Contact,
        [Description("{0} was killed by {1}")] EntityAttack,
        [Description("{0} was shot by {1}")] Projectile,
        [Description("{0} suffocated in a wall")] Suffocation,
        [Description("{0} hit the ground too hard")] Fall,
        [Description("{0} went up in flames")] Fire,
        [Description("{0} burned to death")] FireTick,
        [Description("{0} tried to swim in lava")] Lava,
        [Description("{0} drowned")] Drowning,
        [Description("{0} blew up")] BlockExplosion,
        [Description("{0} blew up")] EntityExplosion,
        [Description("{0} fell out of the world")] Void,
        [Description("{0} died")] Suicide,
        [Description("{0} died magically")] Magic,
        [Description("{0} died a customized death")] Custom
    }
}