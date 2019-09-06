﻿/* 
 * Copyright (c) 2019, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/FirelyTeam/fhir-net-api/master/LICENSE
 */

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Validation.Schema;

namespace Hl7.Fhir.Validation.Impl
{
    public class MaxLength : SimpleAssertion
    {
        private readonly int _maxLength;

        public MaxLength(int maxLength)
        {
            if (maxLength <= 0)
                throw new IncorrectElementDefinitionException($"{nameof(maxLength)}: Must be a positive number");

            _maxLength = maxLength;
        }

        protected override string Key => "maxLength";

        protected override object Value => _maxLength;

        public override Assertions Validate(ITypedElement input, ValidationContext vc)
        {
            if (input == null) throw Error.ArgumentNull(nameof(input));

            if (input.Value != null)
            {
                //TODO: Is ToString() really the right way to turn (Fhir?) Primitives back into their original representation?
                //If the source is POCO, hopefully FHIR types have all overloaded ToString() 
                var serializedValue = input.Value.ToString();

                if (serializedValue.Length > _maxLength)
                {

                    return new Assertions(new ResultAssertion(ValidationResult.Failure, this));
                }
                //return new Assertions(Assertions.Failure + new TraceText($"Value '{serializedValue}' is too long (maximum length is {_maxLength})"));
            }
            return Assertions.Success;
        }
    }
}
