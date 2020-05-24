const express = require("express");
let router = express.Router();

router.get('/', (req, resp) => {
  resp.send('ASD');
})
module.exports = router;
