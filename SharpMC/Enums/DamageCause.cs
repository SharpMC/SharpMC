#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Enums
{
	using System.ComponentModel;

	public enum DamageCause
	{
		[Description("{0} went MIA")]
		Unknown, 

		[Description("{0} was pricked to death")]
		Contact, 

		[Description("{0} was killed by {1}")]
		EntityAttack, 

		[Description("{0}  was shot by {1}")]
		Projectile, 

		[Description("{0} suffocated in a wall")]
		Suffocation, 

		[Description("{0} hit the ground too hard")]
		Fall, 

		[Description("{0} went up in flames")]
		Fire, 

		[Description("{0} burned to death")]
		FireTick, 

		[Description("{0} tried to swim in lava")]
		Lava, 

		[Description("{0} drowned")]
		Drowning, 

		[Description("{0} blew up")]
		BlockExplosion, 

		[Description("{0} blew up")]
		EntityExplosion, 

		[Description("{0} fell out of the world")]
		Void, 

		[Description("{0} died")]
		Suicide, 

		[Description("{0} died magically")]
		Magic, 

		[Description("{0} died a customized death")]
		Custom
	}
}