# Not Enough Accuracy

Do you think XPerfect is still not as strict as you wish?

Not Enough Accuracy (NEAcc. or NEA) take every millisecond into calculation, giving you the most precise accuracy of
your gameplay!

~~Autoplay only scored approximately 98% NE-Acc. on my PC.~~

**This mod does not affect the judgments of ADOFAI!**
**It is just another way of calculating accuracy!**

## Calculation Method

`Tiles` = the total count of tiles, where each mid-spin is counted as only one tile
> i.e. `Tiles` = VE + EP + PP + LP + VL + FailMiss - Midspins

For each tile, the score of the tile is (consider only the judgment that pushes you forward):

* `-100` if no fail is on and it is missed
* `max(100-abs(x), 0)` where x is the deviation from the precise timing of the tile, in milliseconds, no matter what the
  actual judgment is.

> i.e. The base score for each tile is 100. Every millisecond of deviation from the precise timing costs 1 score, up to
> 100 scores. If you miss a tile in no fail mode, its score will be -100.

Every TooEarly and TooLate costs an extra 50 scores.

Every FailOverload costs an extra 100 scores.

`TotalScores` = sum up the scores above (can be negative)

`NEAccuracy% = TotalScores / Tiles`, in percentage

## License

[GPL v3](LICENSE)
