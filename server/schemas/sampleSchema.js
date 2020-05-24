const Joi = require('@hapi/joi');

export default Joi.object().keys(
  {
    something: Joi.string()
  }
);