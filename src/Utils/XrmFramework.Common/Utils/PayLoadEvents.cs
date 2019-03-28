// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum PayLoadEvents
    {
        //EventName
        Create,
        Update,
        Delete,
        Deactivate,
        Activate,
        ReActivate,
        AssignAssistantRole,
        MoveUserFromNegoToDirector,
        ChangeUserBusinessUnit,
        ChangeInEmailToConsolidate,
        Assignment,
        UpdateCDCR0,
        AddNegociatorRole,
        AddAssistantRole,
        AddDirectorRole,
        Completed,
        Canceled,
        EnvoiDemandeAffectation,
        EnvoiDemandeOption,
        EnvoiDemandeReservation,
        EnvoieLeveeOption,
        EnvoieLeveeReservation
    }
}
