using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using Util;
using VillageGame.Services;
using VillageGame.Services.DailyTasks;
using Web.ResponseStructs;

namespace Web.Api
{
    public partial class WebAPI
    {
        private void ParseResponse(string response)
        {
            Debugging.Log($"WebApi: ParseResponse -> try pasrse: {response}", ColorType.Teal);
            
            
            string timePattern = "\"request\":\\s*\"get_time\".*\"time\":\\s*\"([^\"]+)\"";
            Match timeMatch = Regex.Match(response, timePattern);
            if (timeMatch.Success)
            {
                string dateTimeString = timeMatch.Groups[1].Value;
         
                if (DateTime.TryParse(dateTimeString, out var serverTime))
                {
                    Debugging.Log($"Server get time :{serverTime}");
                    GetServerTimeEvent?.Invoke(serverTime);
                    return;
                }
            }

            if (TryParse<LoadResponesData>(response, out var data) && data.response != null)
            {
                if (data.request == "load_user")
                {
                    var pattern = "\"coins_balance\":(\\d+)";
                    var match = Regex.Match(response, pattern);
                    if (match.Success)
                    {
                        try
                        {
                            int coinsBalance = Convert.ToInt32(match.Groups[1].Value);
                            data.response.data.coins_balance = coinsBalance;
                        }
                        catch (Exception e)
                        {
                            //
                        }
                    }

                    Debugging.Log($"User loaded data {true}", ColorType.Teal);
                    LoadData?.Invoke(data.response);
                    return;
                }

                if (data.request == "issue_promocode")
                {
                    if (TryParse<PromocodeResponse>(response, out var promocodeResponse))
                    {
                        if (promocodeResponse.error is "" or " " or null)
                        {
                            PromoCodeService.PromoCodeReceived(promocodeResponse.response.promocode);
                            return;
                        }
                        else
                        {
                            Debugging.Log($"Server error promocode:{promocodeResponse.error}");
                            PromoCodeService.PromoCodeError();
                            return;
                        }
                    }

                    Debugging.Log("Error parsing issue_promocode");
                }

                if (data.request == "task_done")
                {
                    if (TryParse<TaskDoneResponse>(response, out var taskDoneResponse))
                    {
                        if (taskDoneResponse.error is "" or " " or null)
                        {
                            RealLifeService.TaskDoneResponse(taskDoneResponse.response.task_id);
                        }
                        else
                        {
                            Debugging.Log($"Server error task_done:{taskDoneResponse.error}");
                            return;
                        }
                    }

                    Debugging.Log("Error parsing task_done");
                }
            }

            if (TryParse<StandartResponse>(response, out var standardResponse))
            {
                if (standardResponse.error is "" or " " or null)
                {
                    StandardResponseSuccess(standardResponse);
                }
                else
                {
                    StandardResponseError(standardResponse);
                }

                return;
            }

            Debugging.Log($"Parse failed:   {response}", ColorType.Red);
        }

        private void StandardResponseSuccess(StandartResponse response)
        {
            Debugging.Log($"Parse ok", ColorType.Green);

            if (response.request == "remove_coins")
            {
                DodoCoinService.WithdrawSuccess();
            }

            if (response.request == "add_coins")
            {
                DodoCoinService.AddCoinsSuccess();
            }

            if (response.request == "issue_promocode")
            {
                PromoCodeService.PromoCodeReceived(response.response);
                Debug.Log(response);
            }
        }

        private void StandardResponseError(StandartResponse response)
        {
            if (response.request == "remove_coins")
            {
                DodoCoinService.WithdrawError();
            }

            if (response.request == "add_coins")
            {
                DodoCoinService.AddCoinsError();
            }

            if (response.request == "issue_promocode")
            {
                PromoCodeService.PromoCodeError();
            }

            Debugging.Log($"Server error => {response.error}", ColorType.Red);
        }

        private bool TryParse<T>(string response, out T data) where T : class
        {
            data = JsonConvert.DeserializeObject<T>(response);
            return data != null;
        }
    }
}