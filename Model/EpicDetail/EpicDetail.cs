using System.ComponentModel.DataAnnotations.Schema;

namespace IGApi.Model
{
    [Table("epic_detail")]
    public partial class EpicDetail
    {

        [Column("epic")]
        public string Epic { get; set; }

        [Column("expiry")]
        public string Expiry { get; set; } = "-";

        [Column("name")]
        public string Name { get; set; } = Constants.PropertyNullError;

        [Column("force_open_allowed")]
        public bool ForceOpenAllowed { get; set; }

        [Column("stop_limit_allowed")]
        public bool StopsLimitsAllowed { get; set; }

        [Column("lot_size")]
        public decimal? LotSize { get; set; }

        [Column("position_size_unit")]
        public string PositionSizeUnit { get; set; } = Constants.PropertyNullError;

        [Column("type")]
        public string Type { get; set; } = Constants.PropertyNullError;

        [Column("controlled_risk_allowed")]
        public bool ControlledRiskAllowed { get; set; }

        [Column("streaming_prices_available")]
        public bool StreamingPricesAvailable { get; set; }

        [Column("market_id")]
        public string? MarketId { get; set; } = Constants.PropertyNullError;

        [Column("sprintmarket_minimum_expiry_time")]
        public int SprintMarketsMinimumExpiryTime { get; set; }

        [Column("sprintmarket_maximum_expiry_time")]
        public int SprintMarketsMaximumExpiryTime { get; set; }

        [Column("margin_factor")]
        public decimal? MarginFactor { get; set; }

        [Column("margin_factor_unit")]
        public string MarginFactorUnit { get; set; } = Constants.PropertyNullError;

        [Column("news_code")]
        public string? NewsCode { get; set; } = Constants.PropertyNullError;

        [Column("chart_code")]
        public string? ChartCode { get; set; }

        [Column("country")]
        public string? Country { get; set; }

        [Column("value_of_one_pip")]
        public string? ValueOfOnePip { get; set; } = Constants.PropertyNullError;

        [Column("one_pip_means")]
        public string? OnePipMeans { get; set; } = Constants.PropertyNullError;

        [Column("contract_size")]
        public decimal? ContractSize { get; set; }

        [Column("api_last_update")]
        public DateTime ApiLastUpdate { get; set; } = DateTime.UtcNow;

        #region Class contained properties
        #region ExpirtyDetails
        [Column("expiry_last_dealingdate")]
        public DateTime? ExpirylastDealingDate { get; set; }

        [Column("expiry_settlement_info")]
        public string? ExpirysettlementInfo { get; set; }
        #endregion

        #region SlippageFactor
        [Column("slippage_factor_unit")]
        public string? SlippageFactorUnit { get; set; }

        [Column("slippage_factor_value")]
        public decimal? SlippageFactorValue { get; set; }
        #endregion

        #region RolloverDetails
        [Column("last_rollover_time")]
        public DateTime? LastRolloverTime { get; set; }

        [Column("rollover_info")]
        public string? RolloverInfo { get; set; }
        #endregion

        #region DealingRules
        [Column("dealing_rule_unit_min_step_distance")]
        public string? DealingRuleUnitMinStepDistance { get; set; }

        [Column("dealing_rule_unit_min_deal_size")]
        public string? DealingRuleUnitMinDealSize { get; set; }
        
        [Column("dealing_rule_unit_min_controlled_risk_stop_distance")]
        public string? DealingRuleUnitMinControlledRiskStopDistance { get; set; }
        
        [Column("dealing_rule_unit_min_normal_stop_or_limit_distance")]
        public string? DealingRuleUnitMinNormalStopOrLimitDistance { get; set; }
        
        [Column("dealing_rule_unit_max_stop_or_limit_distance")]
        public string? DealingRuleUnitMaxStopOrLimitDistance { get; set; }
        
        [Column("dealing_rule_value_min_step_distance")]
        public decimal? DealingRuleValueMinStepDistance { get; set; }
        
        [Column("dealing_rule_value_min_deal_size")]
        public decimal? DealingRuleValueMinDealSize { get; set; }
        
        [Column("dealing_rule_value_min_controlled_risk_stop_distance")]
        public decimal? DealingRuleValueMinControlledRiskStopDistance { get; set; }
        
        [Column("dealing_rule_value_min_normal_stop_or_limit_distance")]
        public decimal? DealingRuleValueMinNormalStopOrLimitDistance { get; set; }
        
        [Column("dealing_rule_value_max_stop_or_limit_distance")]
        public decimal? DealingRuleValueMaxStopOrLimitDistance { get; set; }
        
        [Column("dealing_rule_market_order_preference")]
        public string? DealingRuleMarketOrderPreference { get; set; }
        
        [Column("dealing_rule_trailing_stop_preference")]
        public string? DealingRuleTrailingStopsPreference { get; set; }
        #endregion
        #endregion

        #region Childs
        public virtual List<EpicDetailMarginDepositBand>? MarginDepositBands { get; set; }

        public virtual List<EpicDetailOpeningHour>? OpeningHours { get; set; }

        public virtual List<EpicDetailSpecialInfo>? SpecialInfo { get; set; }

        public virtual List<EpicDetailCurrency>? Currencies { get; set; }
        #endregion

        //	Linked
        public virtual List<WatchlistEpicDetail> watchlistEpicDetails { get; set; }
    }
}

