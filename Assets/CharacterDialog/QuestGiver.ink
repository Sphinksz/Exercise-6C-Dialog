VAR ratsKilled = 0
VAR questGiven = 0

== beginning ==
{ratsKilled >= 10: ->questCompleted }
{ratsKilled < 10: Hello Brave Adventurer! -> choices }

== choices ==
 +[Wave goodbye] Have a great day!
 --- ->END
 + Have anything you need help with?
 --- {questGiven == 0: ->giveQuest }
 --- {questGiven == 1 && ratsKilled != 10: Come back when you've solved my rat problem! ->choices }
 
 
 ==giveQuest==
 
    --- I need you to kill 10 rats. 
    ~questGiven = 1
    ->choices
    
==questCompleted==
Thank you for taking care of my rat problem! I have nothing else I can offer you. Have a great day! ->END