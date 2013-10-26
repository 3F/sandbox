<?php

/*
  * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
  * Distributed under the Boost Software License, Version 1.0
  * (see accompanying file LICENSE or a copy at http://www.boost.org/LICENSE_1_0.txt)
 */

namespace reg\utility\jsonrender;

require_once 'jsonrender.php';

$tpl = new Formatter(
            "%s [Top-OS: %s: %s% / Top-Countries: %s: %s%]", 
            array(
                "total", 
                'summaries.os.top', 
                'summaries.os.percent', 
                'summaries.geo.top', 
                'summaries.geo.percent'
            )
       );

$json = new JsonData();
$json->fromString($tpl, '{"period": "weekly", "total": 82, "summaries": {"os": {"top": "Windows", "percent": 53}, "geo": {"top": "Spain", "percent": 46}}}');
//$json->fromUrl($tpl, "http://sourceforge.net/projects/<name>/files/stats/json?start_date=2013-09-18&end_date=2013-10-21");
//echo $json->getData();
$json->renderSimple(new TTFParam());