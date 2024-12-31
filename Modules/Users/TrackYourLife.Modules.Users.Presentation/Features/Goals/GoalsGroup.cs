using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals
{
    public class GoalsGroup : Group
    {
        public GoalsGroup()
        {
            Configure(
                ApiRoutes.Goals,
                ep =>
                {
                    ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
                }
            );
        }
    }
}
