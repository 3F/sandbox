<?php

/*
  * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
  * Distributed under the Boost Software License, Version 1.0
  * (see accompanying file LICENSE or a copy at http://www.boost.org/LICENSE_1_0.txt)
 */

namespace reg\utility\jsonrender;

require_once 'jsonrender.php';

$tpl = new Formatter(
            ":: %s [Top-OS: %s: %s% / Top-Country: %s: %s%] (%s..)", 
            array(
                "total", 
                'summaries.os.top', 
                'summaries.os.percent', 
                'summaries.geo.top', 
                'summaries.geo.percent',
                'countries(countriesFormat)'
            )
       );

/**
 * callback example
 * @param array $val
 */
function countriesFormat(array $val)
{
    $ret = '';
    $idx = 1;
    foreach($val as $country){
        $ret .= $country[0] . ', ';
        if(++$idx > 5){
            break;
        }
    }
    return $ret;
}

$json = new JsonData();
$json->fromString($tpl, '{"oses": [["Windows", 47], ["Unknown", 38]], "countries": [["Spain", 38], ["Russia", 30], ["Ukraine", 9], ["Belarus", 6], ["Sweden", 2]], "period": "weekly", "total": 85, "summaries": {"os": {"top": "Windows", "percent": 55}, "geo": {"top": "Spain", "percent": 44}}}');
//$json->fromUrl($tpl, "http://sourceforge.net/projects/<name>/files/stats/json?start_date=2013-09-18&end_date=2013-10-21");
//echo $json->getData();
$json->renderSimple(new TTFParam());